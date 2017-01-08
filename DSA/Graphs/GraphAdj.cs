﻿// /Users/amitp/Documents/dsa/DSA/Graphs/MyClass.cs
// amitp
// (c) Amit Patel
// 03-01-2017
using System;
using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography;
using UnionFind;
using Heaps;
using Interfaces;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Graphs
{
	
	public class GraphAdj
	{
		
		public int V
		{
			get;
			set;
		}

		public List<AdjNode>[] adj
		{
			get;
			set;
		}

		public List<Edge> Edges
		{
			get;
			set;
		}

		public GraphAdj(int v)
		{
			this.V = v;
			this.adj = new List<AdjNode>[this.V];
			for (int i = 0; i < this.V; i++)
			{
				this.adj[i] = new List<AdjNode>(); //allocate actual memory
			}
			this.Edges = new List<Edge>();
		}

		public void AddDirectedEdge(int from, int to)
		{
			adj[from].Add(new AdjNode(to));
			this.Edges.Add(new Edge(from,to,true));
		}

		public void AddDirectedEdge(int from, int to, int weight)
		{
			adj[from].Add(new AdjNode(to,weight));
			this.Edges.Add(new Edge(from, to, true, weight));
		}

		public void AddUnDirectedEdge(int from, int to)
		{
			adj[from].Add(new AdjNode(to));
			adj[to].Add(new AdjNode(from));
			this.Edges.Add(new Edge(from, to));
		}

		public void AddUnDirectedEdge(int from, int to, int weight)
		{
			adj[from].Add(new AdjNode(to,weight));
			adj[to].Add(new AdjNode(from,weight));
			this.Edges.Add(new Edge(from, to, false, weight));
		}

		public List<int> BFSTraversal()
		{
			return BFSTraversal(0);
		}

		public List<int> BFSTraversal(int startNode)
		{
			bool[] visited = new bool[this.V];
			List<int> bfsTraversalList = new List<int>();

			BFSTraversalInternalIterative(startNode, visited, bfsTraversalList);

			for (int i = 0; i < this.V; i++)
			{
				if (i != startNode) // don't process tree having start node again
				{
					if (visited[i] == false)
					{
						BFSTraversalInternalIterative(i,visited,bfsTraversalList);
					}
				}
			}
			return bfsTraversalList;
		}

		public void BFSTraversalInternalIterative(int startNode,bool[] visited, List<int> bfsTraversalList)
		{
			if (startNode >= V)
				return;
			
			Queue<int> queue = new Queue<int>();
			queue.Enqueue(startNode);
			visited[startNode] = true; // mark as visited while enqueu, not dequeue to avoid infinite loop due to self loop 

			while (queue.Count>0)
			{
				int currentVertex = queue.Dequeue();
				bfsTraversalList.Add(currentVertex);

				foreach (var adjacentVertex in adj[currentVertex])
				{
					if (visited[adjacentVertex.Id] == false)
					{
						queue.Enqueue(adjacentVertex.Id);
						visited[adjacentVertex.Id] = true; //to avoid self loops, mark it as visited
					}
				}
			}
		}

		public List<int> DFSTraversal()
		{
			return DFSTraversal(0); //start from zero by default
		}

		public List<int> DFSTraversal(int startNode)
		{
			if (startNode >= V)
				return null;
			
			List<int> dfsTraversalList = new List<int>();
			bool[] visited = new bool[this.V];

			//loop over all vertex in case if there are disconnected trees

			DFSTraversalInternalUtil(startNode, visited, dfsTraversalList);

			for (int i = 0; i < this.V; i++)
			{
				if (i != startNode) // don't process tree having start node
				{
					if (visited[i] == false)
					{
						DFSTraversalInternalUtil(i, visited, dfsTraversalList);
					}
				}
			}

			return dfsTraversalList;
		}

		private void DFSTraversalInternalUtil(int currentVertex, bool[] visited, List<int> dfsTraversalList)
		{
			if (visited[currentVertex] == true)
				return;

			visited[currentVertex] = true;
			dfsTraversalList.Add(currentVertex);

			foreach (var adjacentVertex in adj[currentVertex])
			{
				if (visited[adjacentVertex.Id]  == false) //not visited yet
				{
					DFSTraversalInternalUtil(adjacentVertex.Id, visited, dfsTraversalList);
				}
			}
		}

		public int GetConnectedComponentCountUsingDFSLogic()
		{
			int count = 0;
			bool[] visited = new bool[this.V];

			//loop over all vertex in case if there are disconnected trees, we want to increment components

			for (int i = 0; i < this.V; i++)
			{
				if (visited[i] == false)
				{
					count++;
					GetConnectedComponentCountUsingDFSLogicInternalUtil(i, visited);
				}
			}

			return count;
		}

		private void GetConnectedComponentCountUsingDFSLogicInternalUtil(int currentVertex, bool[] visited)
		{
			if (visited[currentVertex] == true)
				return;

			visited[currentVertex] = true;

			foreach (var adjacentVertex in adj[currentVertex])
			{
				if (visited[adjacentVertex.Id] == false) //not visited yet
				{
					GetConnectedComponentCountUsingDFSLogicInternalUtil(adjacentVertex.Id, visited);
				}
			}
		}

		public bool IsReachableUsingDFSLogic(int from, int to)
		{
			bool[] visited = new bool[this.V];
			bool isReachable = IsReachableUsingDFSLogicInternalUtil(from,to,visited);
			return isReachable;
		}

		private bool IsReachableUsingDFSLogicInternalUtil(int currentVertex, int targetedVertex, bool[] visited)
		{
			if (visited[currentVertex] == true)
				return false;
			
			if (currentVertex == targetedVertex)
				return true;
			
			visited[currentVertex] = true;

			foreach (var adjacentVertex in adj[currentVertex])
			{
				if (visited[adjacentVertex.Id] == false)
				{
					bool isReachable = IsReachableUsingDFSLogicInternalUtil(adjacentVertex.Id,targetedVertex,visited);
					if (isReachable == true)
						return true;
				}
			}

			return false;
		}

		public bool IsCyclicUsingDFSTraversalLogic()
		{
			bool[] visited = new bool[this.V];
			bool[] stillInProgressAndPresentInRecurionStack = new bool[this.V];

			for (int i = 0; i < this.V; i++)
			{
				bool cycleInThisSubtree = IsCyclicUsingDFSTraversalLogicInternalUtil(i, visited, stillInProgressAndPresentInRecurionStack); //start from 0
				if (cycleInThisSubtree == true)
				{
					return true;
				}
			}

			return false;
		}

		private bool IsCyclicUsingDFSTraversalLogicInternalUtil(int currentVertex, bool[] visited, bool[] stillInProgressAndPresentInRecurionStack)
		{
			if (visited[currentVertex] == true)
				return false;

			visited[currentVertex] = true;
			stillInProgressAndPresentInRecurionStack[currentVertex] = true; // mark it gray - in progess

			foreach (var adjacentVertex in adj[currentVertex])
			{
				if (visited[adjacentVertex.Id] == false) // still first time encounter - color white
				{
					bool isCycleInAnyChild = IsCyclicUsingDFSTraversalLogicInternalUtil(adjacentVertex.Id, visited, stillInProgressAndPresentInRecurionStack);
					if (isCycleInAnyChild == true)
					{
						return true;
					}
				}
				else if (stillInProgressAndPresentInRecurionStack[adjacentVertex.Id]==true) // in progress would always be visited, but not vice versa
				{
					return true;
				}
			}

			stillInProgressAndPresentInRecurionStack[currentVertex] = false; // mark it black - completed
			return false;
		}

		public bool IsCyclicUsingUnionFind()
		{
			UnionFindDs unionFind = new UnionFindDs(this.V);

			for (int fromVertex = 0; fromVertex < this.V; fromVertex++)
			{
				foreach (var adjacentVertex in adj[fromVertex])
				{
					//for each edge , we are running it
					int x = unionFind.Find(fromVertex);
					int y = unionFind.Find(adjacentVertex.Id);
					if (x == y) // in same group (only if both are not -1)
					{
						return true; //cycle found
					}
					else
					{
						unionFind.Union(fromVertex,adjacentVertex.Id);
					}
				}
			}
			return false;
		}
		public List<Edge> KruskalMST()
		{
			List<Edge> minimumSpanningTreeEdges = new List<Edge>();
			UnionFindDs unionFindDs = new UnionFindDs(this.V);

			//Sort all edges by their weight
			this.Edges.Sort();

			foreach (var edge in this.Edges)
			{
				//If tree is built, all vertex are connected, then no need to check more
				if (minimumSpanningTreeEdges.Count >= this.V - 1)
					break;

				int edgeFromEndPointGroup = unionFindDs.Find(edge.From);
				int edgeToEndPointGroup = unionFindDs.Find(edge.To);

				if (edgeFromEndPointGroup != edgeToEndPointGroup)
				{
					minimumSpanningTreeEdges.Add(edge);
					unionFindDs.Union(edge.From, edge.To);
				}
			}

			return minimumSpanningTreeEdges;
		}

		public List<Edge> PrimsMST()
		{
			List<Edge> minimumSpanningTreeEdges = new List<Edge>(); // for output

			VertexNode[] vertexDistanceKeyTracker = new VertexNode[this.V];
			Dictionary<int, VertexNode> hashTable = new Dictionary<int, VertexNode>();

			//For others, make it infinite
			for (int i = 0; i < this.V; i++)
			{
				vertexDistanceKeyTracker[i] = new VertexNode(i, Int32.MaxValue, 0);
				hashTable.Add(i, vertexDistanceKeyTracker[i]);
			}

			vertexDistanceKeyTracker[0].Key = 0;
			vertexDistanceKeyTracker[0].Parent = -1; //start node distance key = zero, parent -1

			//create new min heap to get minimum of all adjacent
			Heap<VertexNode> minHeap = new Heap<VertexNode>(vertexDistanceKeyTracker, HeapType.MinHeap);

			//Take v-1 edges, or v vertex
			while (!minHeap.IsEmpty())
			{
				VertexNode minimumDistanceNode = minHeap.GetZeroIndexElement();
				minHeap.RemoveZeroIndexElement();

				//For output, add retrieved node with its parent
				if (minimumDistanceNode.Parent != -1)
				{
					minimumSpanningTreeEdges.Add(new Edge(minimumDistanceNode.Parent,
														  minimumDistanceNode.Id,
														  false,
														  minimumDistanceNode.Key));
				}

				int currentVertex = minimumDistanceNode.Id;
				foreach (var adjacentVertex in adj[currentVertex])
				{
					VertexNode adjacentVertexNode = hashTable[adjacentVertex.Id];
					
					if (minHeap.IsInHeap(adjacentVertexNode) &&
					    adjacentVertex.EdgeWeight < adjacentVertexNode.Key)
					{
						//To update parent, we will need node itself, not just id
						adjacentVertexNode.Parent = currentVertex;
						adjacentVertexNode.Key = adjacentVertex.EdgeWeight; // only equal

						//CRITICAL - Need to re heapify
						minHeap.UpdateHeapForChangedPriority(adjacentVertexNode);
					}
				}
			}

			return minimumSpanningTreeEdges;
		}

		public Dictionary<int, int> DijkstraShortestPathFromSource(int source)
		{
			Dictionary<int, int> vertexToShortestDistance = new Dictionary<int, int>();

			VertexNode[] vertexDistanceKeyTracker = new VertexNode[this.V];
			Dictionary<int, VertexNode> hashTable = new Dictionary<int, VertexNode>();

			//For others, make it infinite
			for (int i = 0; i < this.V; i++)
			{
				vertexDistanceKeyTracker[i] = new VertexNode(i, 999, 0);
				hashTable.Add(i, vertexDistanceKeyTracker[i]);
			}

			vertexDistanceKeyTracker[source].Key = 0;

			//create new min heap to get minimum of all adjacent
			Heap<VertexNode> minHeap = new Heap<VertexNode>(vertexDistanceKeyTracker, HeapType.MinHeap);

			while (!minHeap.IsEmpty())
			{
				VertexNode minimumDistanceNode = minHeap.GetZeroIndexElement();
				minHeap.RemoveZeroIndexElement();

				//For output
				vertexToShortestDistance.Add(minimumDistanceNode.Id,minimumDistanceNode.Key);

				int currentVertex = minimumDistanceNode.Id;
				VertexNode currentVertexNode = hashTable[currentVertex];
					
				foreach (var adjacentVertex in adj[currentVertex])
				{
					VertexNode adjacentVertexNode = hashTable[adjacentVertex.Id];

					if (minHeap.IsInHeap(adjacentVertexNode) &&
					    currentVertexNode.Key != 999 &&
						currentVertexNode.Key + adjacentVertex.EdgeWeight < adjacentVertexNode.Key) //change in condition
					{
						adjacentVertexNode.Key = currentVertexNode.Key + adjacentVertex.EdgeWeight; // plus current

						//CRITICAL - Need to re heapify
						minHeap.UpdateHeapForChangedPriority(adjacentVertexNode);
					}
				}
			}
			return vertexToShortestDistance;
		}

		public Dictionary<int, int> BellmanFordShortestPathWithNegativeCycles(int source, ref bool negativeCycleFlag)
		{
			Dictionary<int, int> vertexToShortestDistance = new Dictionary<int, int>();
			int[] distance = new int[this.V];

			for (int i = 0; i < this.V; i++)
			{
				distance[i] = Int32.MaxValue;
			}
			distance[source] = 0; //initialize source distance as zero

			//Loop for v-1 times, because simple path can have at most v-1 edges
			for (int i = 0; i < V-1; i++)
			{
				for (int edgeNumber = 0; edgeNumber < this.Edges.Count; edgeNumber++)
				{
					bool flag = CheckIfDistanceNeedsToBeUpdated(distance,edgeNumber);
					if (flag == true)
					{
						distance[this.Edges[edgeNumber].To] = distance[this.Edges[edgeNumber].From] + 
																	this.Edges[edgeNumber].Weight;
					}
				}
			}

			//When we have processed edges v-1 times, there should not be any more scope for less distance
			//If we still find any such less distance, then it would be surely negative cycle.
			for (int edgeNumber = 0; edgeNumber < this.Edges.Count; edgeNumber++)
			{
				bool flag = CheckIfDistanceNeedsToBeUpdated(distance, edgeNumber);
				if (flag == true)
				{
					//FOUND NEGATIVE WEIGHT CYCLE
					negativeCycleFlag = true;
				}
			}

			//Output distance as dictionary
			if (negativeCycleFlag == false)
			{
				for (int i = 0; i < this.V; i++)
				{
					vertexToShortestDistance.Add(i,distance[i]);
				}
			}

			return vertexToShortestDistance;
		}

		private bool CheckIfDistanceNeedsToBeUpdated(int[] distance, int edgeNumber)
		{
			int source = this.Edges[edgeNumber].From;
			int destination = this.Edges[edgeNumber].To;
			int edgeWeight = this.Edges[edgeNumber].Weight;

			if (distance[source] != Int32.MaxValue &&
			   distance[source] + edgeWeight < distance[destination])
			{
				return true;
			}
			return false;
		}

		public int[,] FloydWarshallAllPairShortestPaths()
		{
			int[,] solution = new int[this.V, this.V];

			//Initlize all as INF/MAX
			for (int i = 0; i < this.V; i++)
			{
				for (int j = 0; j < this.V; j++)
				{
					if(i!=j)
						solution[i, j] = Int32.MaxValue;
				}
			}

			//Assign current graph layout to solution in matrix form

			for (int currentVertex= 0; currentVertex < this.V; currentVertex++)
			{
				foreach (var adjacentVertex in adj[currentVertex])
				{
					solution[currentVertex, adjacentVertex.Id] = adjacentVertex.EdgeWeight;
				}
			}

			//Iterate through all vertex as intermediate path
			for (int intermediateVertex = 0; intermediateVertex < this.V; intermediateVertex++)
			{
				for (int fromVertex = 0; fromVertex < this.V; fromVertex++)
				{
					for (int toVertex = 0; toVertex < this.V; toVertex++)
					{
						int currentDirectDistance = solution[fromVertex, toVertex];
						int distanceUsingIntermediate = Int32.MaxValue;

						//To avoid interger overflow when summing to max values, count it seperatly
						if (solution[fromVertex, intermediateVertex] < Int32.MaxValue / 2 &&
						   solution[intermediateVertex, toVertex] < Int32.MaxValue / 2)
						{
							distanceUsingIntermediate = solution[fromVertex, intermediateVertex] + solution[intermediateVertex, toVertex];
						}

						//Update if needed
						if (distanceUsingIntermediate < currentDirectDistance)
						{
							solution[fromVertex, toVertex] = distanceUsingIntermediate;
						}
					}
				}
			}

			return solution;
		}

		public bool[,] FloydWarshallAllPairReachabilityMatrixOrTransitiveClosure()
		{
			bool[,] solution = new bool[this.V, this.V];

			//Initlize all as INF/MAX
			for (int i = 0; i < this.V; i++)
			{
				for (int j = 0; j < this.V; j++)
				{
					if (i != j)
						solution[i, j] = false;
					else
						solution[i, j] = true;
				}
			}

			//Assign current graph layout to solution in matrix form
			for (int currentVertex = 0; currentVertex < this.V; currentVertex++)
			{
				foreach (var adjacentVertex in adj[currentVertex])
				{
					solution[currentVertex, adjacentVertex.Id] = true;
				}
			}

			//Iterate through all vertex as intermediate path
			for (int intermediateVertex = 0; intermediateVertex < this.V; intermediateVertex++)
			{
				for (int fromVertex = 0; fromVertex < this.V; fromVertex++)
				{
					for (int toVertex = 0; toVertex < this.V; toVertex++)
					{
						solution[fromVertex, toVertex] = solution[fromVertex, toVertex] ||
							(solution[fromVertex, intermediateVertex] && solution[intermediateVertex, toVertex]);
					}
				}
			}

			return solution;
		}
	}
}

