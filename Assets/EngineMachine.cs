using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineMachine : MachineController
{
    public Light reactionGlow;
    public LinearTranslation engineFlapTranslationOne;
    public LinearTranslation engineFlapTranslationTwo;


    private StatusPoleLightColor _previousStatus;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        _previousStatus = statusPole.statusColor;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if(statusPole.statusColor != _previousStatus)
        {
            _previousStatus = statusPole.statusColor;

            reactionGlow.enabled = statusPole.statusColor != StatusPoleLightColor.Error;
            engineFlapTranslationOne.enabled = statusPole.statusColor != StatusPoleLightColor.Error;
            engineFlapTranslationTwo.enabled = statusPole.statusColor != StatusPoleLightColor.Error;
        }
    }
}
