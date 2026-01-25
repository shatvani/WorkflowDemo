using System;
using System.Collections.Generic;
using System.Text;
using Workflow.Models;

namespace Workflow.Storage;

public interface IProcessDefinitionStore
{
    Task<ProcessDefinition> Load(string processId, int version, CancellationToken ct = default);
}
