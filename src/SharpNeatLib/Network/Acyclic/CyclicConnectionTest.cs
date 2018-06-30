﻿using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Redzen;
using Redzen.Collections;
using Redzen.Structures;

namespace SharpNeat.Network.Acyclic
{
    // TODO: Unit tests.

    /// <summary>
    /// For testing if a new connection would form a connectivity cycle in an existing acyclic digraph.
    /// </summary>
    /// <remarks>
    /// Also see:
    /// <see cref="SharpNeat.Neat.Reproduction.Asexual.Strategy.CyclicConnectionTest"/>
    /// <see cref="SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover.CyclicConnectionTest"/>
    /// <see cref="SharpNeat.Network.Acyclic.AcyclicGraphDepthAnalysis"/>
    /// <see cref="SharpNeat.Network.CyclicGraphAnalysis"/>
    /// </remarks>
    public class CyclicConnectionTest
    {
        #region Instance Fields

        /// <summary>
        /// The graph traversal stack, as required by a depth first graph traversal algorithm.
        /// Each stack entry is an index into a connection list, representing both the current node being traversed 
        /// (the connections's source ID), and the current position in that node's outgoing connections.
        /// </summary>
        IntStack _traversalStack = new IntStack(16);

        /// <summary>
        /// A bitmap in which each bit represents a node in the graph. 
        /// The set bits represent the set of visited nodes on the current traversal path.
        /// This is used to quickly determine if a given path should be traversed or not. 
        /// </summary>     
        BoolArray _visitedNodeBitmap = new BoolArray(1024);

        #if DEBUG
        /// <summary>
        /// Indicates if a call to IsConnectionCyclic() is currently in progress.
        /// For checking for attempts to re-enter that method while a call is in progress.
        /// </summary>
        int _reentranceFlag = 0;
        #endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests if the proposed new connection newConn would form a cycle if added to the existing directed
        /// acyclic graph described by digraph.
        /// </summary>
        /// <param name="digraph">The directed acyclic graph to run the test against.</param>
        /// <param name="newConn">A proposed new connection to add to the graph.
        /// Note. the connection source and target nodes IDs are node indexes as used by the supplied digraph.</param>
        public bool IsConnectionCyclic(
            DirectedGraph digraph,
            DirectedConnection newConn)
        {
            #if DEBUG
            // Check for attempts to re-enter this method.
            if(1 == Interlocked.CompareExchange(ref _reentranceFlag, 1, 0)) {
                throw new InvalidOperationException("Attempt to re-enter non reentrant method.");
            }
            #endif

            EnsureNodeCapacity(digraph.TotalNodeCount);
            
            try 
            {
                return IsConnectionCyclicInner(digraph, newConn);
            }
            finally 
            {   // Ensure cleanup occurs before we return so that we can guarantee the class instance is ready for 
                // re-use on the next call.
                Cleanup();
            }
        }

        #endregion

        #region Private Methods

        private bool IsConnectionCyclicInner(DirectedGraph digraph, DirectedConnection newConn)
        {
            // Test if the new connection is pointing to itself.
            if(newConn.SourceId == newConn.TargetId) {
                return true;
            }

            // Note. We traverse forwards starting at the new connection's target node. If the new connection's source node is
            // encountered during traversal then the connection would form a cycle in the graph as a whole, and we return true.
            int startNodeId = newConn.TargetId;

            // Search for outgoing connections from the starting node.
            int connIdx = digraph.GetFirstConnectionIndex(startNodeId);
            if(connIdx < 0)
            {   // The current node has no outgoing connections, therefore newConn does not form a cycle.
                return false;
            }

            // Initialise and run the graph traversal algorithm.
            InitGraphTraversal(startNodeId, connIdx);

            // Note. we pass newConn.SourceId as the terminalNodeId; if traversal reaches this node then a cycle has been detected.
            return TraverseGraph(digraph, newConn.SourceId);
        }

        private void Cleanup()
        {
            _traversalStack.Clear();
            _visitedNodeBitmap.Reset(false);

            #if DEBUG
            // Reset reentrancy test flag.
            Interlocked.Exchange(ref _reentranceFlag, 0);
            #endif
        }

        private void EnsureNodeCapacity(int capacity)
        {
            if (capacity > _visitedNodeBitmap.Length)
            {
                // For the new capacity, select the lowest power of two that is above the required capacity.
                capacity = MathUtils.CeilingToPowerOfTwo(capacity);

                // Allocate new bitmap with the new capacity.
                _visitedNodeBitmap = new BoolArray(capacity);
            }
        }

        #endregion

        #region Private Methods [Graph Traversal]

        private void InitGraphTraversal(int startNodeId, int connIdx)
        {
            // Push connIdx onto the stack.
            _traversalStack.Push(connIdx);

            // Add the current node to the set of visited nodes; this prevents the traversal algorithm from re-entering this node 
            // (it's on the stack thus it is in the process of being traversed).
            _visitedNodeBitmap[startNodeId] = true;
        }

        /// <summary>
        /// The graph traversal algorithm.
        /// </summary>
        /// <param name="digraph">The directed acyclic graph to traverse.</param>
        /// <param name="terminalNodeId">// The 'terminal' node ID, i.e. if traversal reaches this node 
        /// then newConn would form a cycle and we stop/terminate traversal.</param>
        /// <returns></returns>
        private bool TraverseGraph(DirectedGraph digraph, int terminalNodeId)
        {
            int[] srcIdArr = digraph.ConnectionIdArrays._sourceIdArr;
            int[] tgtIdArr = digraph.ConnectionIdArrays._targetIdArr;

            // While there are entries on the stack.
            while(0 != _traversalStack.Count)
            {
                // Get the connection index from the top of stack; this is the next connection to be traversed.
                int currConnIdx = _traversalStack.Peek();

                // Notes.
                // Before we traverse the current connection, update the stack state to point to the next connection
                // to be traversed from the current node. I.e. set up the stack state ready for when the traversal down 
                // into the current connection completes and returns back to the current node.
                //
                // This is essentially tail call optimisation, and will result in shorter stacks on average and also
                // has the side effect that we can no longer examine the stack to observe the traversal path at a given 
                // point in time.
                MoveForward(srcIdArr, tgtIdArr, currConnIdx);

                // Test if the next traversal child node has already been visited.
                int childNodeId = tgtIdArr[currConnIdx];
                if(_visitedNodeBitmap[childNodeId]) {
                    continue;
                }

                // Test if the connection target is the terminal node.
                if(childNodeId == terminalNodeId) {
                    return true;
                }

                // We're about to traverse into childNodeId, so mark it as visited to prevent re-traversal.
                _visitedNodeBitmap[childNodeId] = true;

                // Search for outgoing connections from childNodeId.
                int connIdx = digraph.GetFirstConnectionIndex(childNodeId);
                if(connIdx >= 0)
                {   // childNodeId has outgoing connections; push the first connection onto the stack to mark it for traversal.
                    _traversalStack.Push(connIdx);    
                }
            }

            // Traversal has completed without visiting the terminal node, therefore the new connection
            // does not form a cycle in the graph.
            return false;
        }

        /// <summary>
        /// Update the stack state to point to the next connection to traverse down.
        /// </summary>
        /// <returns>The current connection to traverse down.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MoveForward(int[] srcIdArr, int[] tgtIdAr, int currConnIdx)
        {
            // If the current node has at least one more outgoing connection leading to an unvisited node,
            // then update the node's entry on the top of the stack to point to said connection.
            for(int i=currConnIdx + 1; i < srcIdArr.Length && (srcIdArr[currConnIdx] == srcIdArr[i]); i++)
            {
                if(!_visitedNodeBitmap[tgtIdAr[i]])
                {
                    _traversalStack.Poke(currConnIdx + 1);
                    return;
                }
            }

            // No more connections for the current node; pop/remove the current node from the top of the stack.
            // Traversal will thus continue from its traversal parent node's current position, or will terminate 
            // if the stack is now empty.
            _traversalStack.Pop();
        }

        #endregion
    }
}
