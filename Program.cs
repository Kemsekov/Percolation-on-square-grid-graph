using System.Collections.Concurrent;
using GraphSharp;
using GraphSharp.Graphs;
using MathNet.Numerics.LinearAlgebra.Single;
using SixLabors.ImageSharp;
using SysColor = System.Drawing.Color;

// grid size
var N = 100;

// Probability of node appearing
var node_probability = 0.6;

// Probability of edge appearing
var edge_probability = 0.8;

// Seed for nodes generation
int? seed = null;

System.Console.WriteLine("Creaing graph...");
Graph graph = CreatePercolationGraph(N, node_probability, edge_probability, seed);

System.Console.WriteLine("Searching components...");
var start = DateTime.Now;
var components = graph.Do.FindComponents();
System.Console.WriteLine($"Ms to find components {(DateTime.Now-start).Microseconds}");
System.Console.WriteLine($"Components {components.Components.Length}");


System.Console.WriteLine("Recolor...");
var all_colors = Enum.GetValues<System.Drawing.KnownColor>().ToArray();
Parallel.ForEach(components.Components, comp =>
{
    var subg = graph.Do.Induce(comp.Select(v => v.Id));
    var i = subg.Nodes.Select(v => v.Id).Max();
    foreach (var e in subg.Edges)
    {
        e.Color = SysColor.FromKnownColor(all_colors[i % all_colors.Length]);
    }
});


System.Console.WriteLine("Creating Image...");
using var image = ImageSharpShapeDrawer.CreateImage(graph, drawer =>
    {
        drawer.Clear(SysColor.Black);
        drawer.DrawEdgesParallel(graph.Edges, 0.002);
        // drawer.DrawNodesParallel(graph.Nodes, 0.008, color: SysColor.Red);
        // drawer.DrawNodeIds(graph.Nodes, SysColor.White, 0.01);
    },
    x => (Vector)(x.MapProperties().Position*0.9f+0.05f),
    outputResolution: 2000
);

System.Console.WriteLine("Saving Image...");
image.SaveAsJpeg("example.jpg");

static Graph CreatePercolationGraph(int N, double node_probability, double edge_probability, int? seed)
{
    var start = DateTime.Now;

    var graph = new Graph();
    graph.Do.CreateNodes(N * N);
    var rnd = new Random();
    if (seed is not null)
    {
        rnd = new Random((int)seed);
    }
    Parallel.For(0, N, i =>
        {
            for (int j = 0; j < N; j++)
            {
                var pos = new DenseVector([1.0f * i / N, 1.0f * j / N]);
                graph.Nodes[i * N + j].MapProperties().Position = pos;
            }
    });
    Parallel.For(0, N, i =>
    {
        var rnd = new Random();
        if (seed is not null)
        {
            rnd = new Random((int)seed+i);
        }
        for (int j = 0; j < N; j++)
        {
            var right_ind = (i + 1) * N + j;
            var bottom_ind = i * N + j + 1;

            var current = i * N + j;

            if (i + 1 < N && rnd.NextSingle() < edge_probability)
            {
                graph.Edges.Add(new Edge(current, right_ind));
            }

            if (j + 1 < N && rnd.NextSingle() < edge_probability)
            {
                graph.Edges.Add(new Edge(current, bottom_ind));
            }
        }
    });

    var toRemove = graph.Nodes.Where(c => rnd.NextSingle() >= node_probability).Select(n => n.Id).ToArray();
    graph.Do.RemoveNodes(toRemove);

    var end = DateTime.Now;
    var diff = end-start;
    System.Console.WriteLine($"Ms to create graph {diff.Microseconds}");
    return graph;
}