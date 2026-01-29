using Workflow.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Workflow.Storage;

public sealed class YamlProcessDefinitionStore : IProcessDefinitionStore
{
    private readonly IDeserializer _deserializer;

    public YamlProcessDefinitionStore()
    {
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public Task<ProcessDefinition> Load(string processId, int version, CancellationToken ct = default)
    {
        var baseDir = AppContext.BaseDirectory;
        var path = Path.Combine(baseDir, "process-definitions", processId, $"v{version}.yaml");

        if (!File.Exists(path))
            throw new FileNotFoundException($"Process definition YAML not found at: {path}");

        var yaml = File.ReadAllText(path);

        var root = _deserializer.Deserialize<YamlRoot>(yaml);

        if (!string.Equals(root.Id, processId, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"YAML id '{root.Id}' does not match requested processId '{processId}'.");

        if (root.Version != version)
            throw new InvalidOperationException($"YAML version '{root.Version}' does not match requested version '{version}'.");

        var def = new ProcessDefinition
        {
            Id = root.Id,
            Version = root.Version,
            StartStepId = root.Start.StepId,
            Steps = root.Steps.ToDictionary(
                s => s.Id,
                s => new StepDef
                {
                    Id = s.Id,
                    TaskType = s.TaskType,
                    Outcomes = new HashSet<string>(s.Outcomes ?? new(), StringComparer.OrdinalIgnoreCase)
                },
                StringComparer.OrdinalIgnoreCase
            ),
            Transitions = root.Transitions.Select(t => new TransitionDef
            {
                From = t.From,
                On = t.On,
                When = t.When,
                To = t.To,
                Note = t.Note
            }).ToList()
        };

        return Task.FromResult(def);
    }

    private sealed class YamlRoot
    {
        public required string Id { get; set; }
        public required int Version { get; set; }
        public required YamlStart Start { get; set; }
        public required List<YamlStep> Steps { get; set; }
        public required List<YamlTransition> Transitions { get; set; }
    }

    private sealed class YamlStart
    {
        public required string StepId { get; set; }
    }

    private sealed class YamlStep
    {
        public required string Id { get; set; }
        public required string TaskType { get; set; }
        public List<string>? Outcomes { get; set; }
    }

    private sealed class YamlTransition
    {
        public required string From { get; set; }
        public string? On { get; set; }
        public string? When { get; set; }
        public required string To { get; set; }
        public string? Note { get; set; }
    }
}
