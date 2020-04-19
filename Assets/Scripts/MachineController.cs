using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineController : MonoBehaviour
{
    public MachineTerminal terminal;
    public StatusPole statusPole;

    public double currentAmount, totalAmount;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        terminal.Clear();
        terminal.AppendLine("Example Status");

        double progress = Math.Max(Math.Min(currentAmount / totalAmount, 1), 0);
        terminal.AppendLine("Status: " + (Math.Round(progress * 100)).ToString() + "%");
        terminal.AppendProgressLine(progress);
    }
}
