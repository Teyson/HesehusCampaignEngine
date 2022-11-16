namespace CampaignEngine.Domain.UndirectedGraph;

/// <summary>
/// Inspiration taken from: https://codereview.stackexchange.com/questions/247435/undirected-graph-data-structure-in-c
/// </summary>
public class UndirectedGraph<T> where T : notnull
{
    // array of adjacency lists
    private Dictionary<T, HashSet<T>> _adjacencyList;

    /// <summary>
    /// </summary>
    /// <param name="vertices">The list of vertices in the graph</param>
    public UndirectedGraph(List<T> vertices)
    {
        // create array of lists
        // initialise all lists to empty
        _adjacencyList = new Dictionary<T, HashSet<T>>();
        foreach (var vertex in vertices)
            _adjacencyList.Add(vertex, new HashSet<T>());
    }

    /// <summary>
    /// Add an edge to the graph.
    /// </summary>
    /// <param name="v">One vertex of the new edge.</param>
    /// <param name="w">The other vertex of the new edge.</param>
    public void AddEdge(T v, T w)
    {
        // add to adjacency lists
        _adjacencyList[v].Add(w);
        _adjacencyList[w].Add(v);
    }

    /// <summary>
    /// Get an adjacency list for a vertex.
    /// </summary>
    /// <param name="v">The vertex.</param>
    /// <returns></returns>
    public HashSet<T> GetAdjacency(T v)
    {
        return _adjacencyList[v];
    }
}