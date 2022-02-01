using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBehaviour : CollectableBehaviour
{
    public override void Collect()
    {
        GameplayManager.instance.GetCard();
        base.Collect();
    }
}
