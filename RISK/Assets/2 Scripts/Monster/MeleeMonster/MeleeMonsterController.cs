using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonsterController : NormalMonster
{
    protected override void InitializeState()
    {
        ChangeState(new MonsterIdle());
    }


}
