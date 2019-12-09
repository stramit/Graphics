using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.Graphing;
using Data.Util;

namespace UnityEditor.ShaderGraph
{
    class ContextManager
    {
        readonly GraphData m_GraphData;

        List<BlockData> m_AllBlocks;
        List<BlockData> m_ValidBlocks;
        bool m_IsActive;
        
        public ContextManager(GraphData graphData)
        {
            m_GraphData = graphData;
            m_AllBlocks = ListPool<BlockData>.Get();
            m_ValidBlocks = ListPool<BlockData>.Get();
        }

        ContextData vertexContext => NodeUtils.DepthFirstCollectContextsFromContext(outputContext, m_GraphData)
            .FirstOrDefault(x => x.contextType.type == typeof(VertexContext));
            
        ContextData fragmentContext => NodeUtils.DepthFirstCollectContextsFromContext(outputContext, m_GraphData)
            .FirstOrDefault(x => x.contextType.type == typeof(FragmentContext));

        ContextData outputContext => m_GraphData.contexts.FirstOrDefault(x => x.contextType.type == typeof(OutputContext));

        public bool isActive => m_IsActive;
        public bool isVertexActive => m_IsActive && vertexContext != null;
        public bool isFragmentActive => m_IsActive && fragmentContext != null;

        public List<BlockData> allBlocks => m_AllBlocks;
        public List<BlockData> validBlocks => m_ValidBlocks;
        
        public BlockData targetBlock => outputContext?.blocks.Where(x => x.GetType() == typeof(TargetBlock)).FirstOrDefault();

        public void Init(bool isActive)
        {
            m_IsActive = isActive;
        }

        // Get all unfiltered data from Contexts
        public void UpdateBlocks()
        {
            m_AllBlocks.Clear();
            var contexts = NodeUtils.DepthFirstCollectContextsFromContext(outputContext, m_GraphData);
            foreach(var context in contexts)
            {
                foreach(var block in context.blocks)
                {
                    m_AllBlocks.Add(block);
                }
            };

            // Always update filter for active blocks
            SetBlocksActiveToFilter();
            DirtyOutput();
        }

        public void DirtyBlock(BlockData blockData, ModificationScope scope = ModificationScope.Graph)
        {
            bool blocksNeedUpdating = false;
            AddBlocksOfType(blockData.requireBlocks, ref blocksNeedUpdating);

            if(blocksNeedUpdating)
            {
                // New Blocks have been added
                // Update block list and filter
                UpdateBlocks();
            }
            else
            {
                // No new Blocks
                // Just update filter
                SetBlocksActiveToFilter();
            }

            DirtyOutput(scope);
        }

        public void DirtyOutput(ModificationScope scope = ModificationScope.Graph)
        {
            targetBlock.Dirty(scope);
        }

        // This must be public because SearcWindowProvider needs to create the BlockData upfront
        // Then pass the instance to the OnSelectEntry call
        public void AddBlocksOfType(Type[] requireBlocks, ref bool blocksNeedUpdating)
        {
            if(requireBlocks == null)
                return;
            
            foreach(Type requiredType in requireBlocks)
            {
                if(!requiredType.IsSubclassOf(typeof(BlockData)))
                    continue;
                
                // Create block
                var blockData = (BlockData)Activator.CreateInstance(requiredType);
                blockData.owner = m_GraphData;

                // Find context
                var foundContexts = NodeUtils.DepthFirstCollectContextsFromContext(outputContext, m_GraphData);
                var context = foundContexts.Where(x => x.contextType.type == blockData.contextType).FirstOrDefault();
                if(context == null)
                    continue;

                // Add block to context
                if(context.blocks.Any(x => x.GetType() == blockData.GetType()))
                    continue;
                
                context.blocks.Add(blockData);
                blocksNeedUpdating = true;

                // Recursively add required
                AddBlocksOfType(blockData.requireBlocks, ref blocksNeedUpdating);
            }
        }

        public void AddRequiredBlocksForImplementations()
        {
            bool blocksNeedUpdating = false;
            foreach(ITargetImplementation implementation in m_GraphData.activeTargetImplementations)
            {
                AddBlocksOfType(implementation.requireBlocks, ref blocksNeedUpdating);
            }

            if(blocksNeedUpdating)
            {
                // New Blocks have been added
                // Update block list and filter
                UpdateBlocks();
            }
            else
            {
                // No new Blocks
                // Just update filter
                SetBlocksActiveToFilter();
            }

            // targetBlock.Dirty(Graphing.ModificationScope.Graph);
        }

        // Filter BlockDatas not supported by the current Implementation
        public void FilterBlocksForImplementation(ITargetImplementation implementation)
        {
            m_ValidBlocks = allBlocks.Where(x => implementation.GetSupportedBlocks(allBlocks).Contains(x.GetType())).ToList();
        }

        public void GetActiveFields(ActiveFields activeFields, ITargetImplementation implementation, PassDescriptor passDescriptor)
        { 
            foreach(var block in validBlocks)
            {
                var fields = GenerationUtils.GetActiveFieldsFromConditionals(block.GetConditionalFields(passDescriptor));
                foreach(FieldDescriptor field in fields)
                    activeFields.baseInstance.Add(field);
            }
        }

        // This is only used for generation preview properties for the master preview
        // As this is calculated at the shader level we do not have access to ITargetImplementation
        // to refine the list to only supported BlockDatas
        public void DepthFirstCollectNodesFromAllBlocks(List<AbstractMaterialNode> nodes)
        {
            foreach(BlockData block in m_AllBlocks)
            {
                NodeUtils.DepthFirstCollectNodesFromNode(nodes, block);
            }
        }

        public void GetFilteredVertexBlocks(List<AbstractMaterialNode> nodeList, ITargetImplementation implementation)
        {
            if(isVertexActive)
            {
                foreach(BlockData block in vertexContext.blocks.Where(x => validBlocks.Contains(x)))
                    NodeUtils.DepthFirstCollectNodesFromNode(nodeList, block);
            }
        }

        public void GetFilteredFragmentBlocks(List<AbstractMaterialNode> nodeList, ITargetImplementation implementation)
        {
            if(isFragmentActive)
            {
                foreach(BlockData block in fragmentContext.blocks.Where(x => validBlocks.Contains(x)))
                    NodeUtils.DepthFirstCollectNodesFromNode(nodeList, block);
            }
        }

        public void GetFilteredVertexSlots(List<MaterialSlot> slotList, ITargetImplementation implementation)
        {
            if(isVertexActive)
            {
                foreach(BlockData block in vertexContext.blocks.Where(x => validBlocks.Contains(x)))
                {
                    block.GetInputSlots(slotList);
                }
            }
        }

        public void GetFilteredFragmentSlots(List<MaterialSlot> slotList, ITargetImplementation implementation)
        {
            if(isFragmentActive)
            {
                foreach(BlockData block in fragmentContext.blocks.Where(x => validBlocks.Contains(x)))
                {
                    block.GetInputSlots(slotList);
                }
            }
        }

        public void SetBlocksActiveToFilter()
        {
            // Build list of currently valid Blocks
            var supportedBlockTypes = ListPool<Type>.Get();
            supportedBlockTypes.Add(typeof(TargetBlock));
            foreach(var implementation in m_GraphData.activeTargetImplementations)
            {
                supportedBlockTypes.AddRange(implementation.GetSupportedBlocks(allBlocks));
            }

            // Set active state
            foreach(var block in allBlocks)
            {
                block.isActive = supportedBlockTypes.Contains(block.GetType());
            }
        }
    }
}
