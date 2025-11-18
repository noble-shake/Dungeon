using System;
using Unity.Behavior;

[BlackboardEnum]
public enum CombatState
{
	ReJudge,
	Judge,
	Move,
	Attack,
	Repositioning,
	Groggy
}
