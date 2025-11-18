using System;
using Unity.Behavior;

[BlackboardEnum]
public enum ImprovedEnemyState
{
	Sleep,
	Idle,
	NormalPattern,
	Dead,
	PhaseShift,
	FirstPattern,
	SecondPattern,
	ThirdPattern,
	CutScenePlaying,
	Groggy,
	NormalPatternWithFixedAggro
}
