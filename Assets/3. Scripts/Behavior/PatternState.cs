using System;
using Unity.Behavior;

[BlackboardEnum]
public enum PatternState
{
    Prepare,
	MainProcess,
	PatternEndConditionCheck,
	PatternEnd
}
