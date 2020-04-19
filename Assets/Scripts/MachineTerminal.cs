using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MachineTerminal : MonoBehaviour
{

    public TextMeshPro terminalText;

    public Material sidePanelMaterial;
    public List<GameObject> sidePanels;

    public List<string> lines;

    public int charsWide;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in sidePanels)
        {
            obj.GetComponent<Renderer>().material = sidePanelMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        terminalText.text = string.Join("\n", lines.ToArray());
    }

    public void Clear()
    {
        lines.Clear();
    }

    public void AppendLine(string line)
    {
        lines.Add(line);
    }

    public void AppendProgressLine(double progress)
    {
        int progressChars = (int)Math.Round((double)(charsWide - 2) * Math.Min(Math.Max(progress, 0), 1));
        int nonProgressChars = (charsWide - 2) - progressChars;

       this.AppendLine("[" + new string('=', progressChars) + new string(' ', nonProgressChars) + "]");
    }
}
