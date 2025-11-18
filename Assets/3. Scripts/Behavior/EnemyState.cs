using System;
using Unity.Behavior;

[BlackboardEnum]
public enum EnemyState
{
	Sleep,
	Idle,
	Chase,
	Attack,
	Pattern1,
	Pattern2,
	Pattern3,
	Hit,
	Dead,
	StanceBreak,
	PhaseShift,
	CutScenePlaying,
	Normal_Pattern
}
