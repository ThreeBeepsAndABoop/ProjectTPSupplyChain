using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    #region Dependencies/editor state.

    public ICollection<GameObject> motors;
    public ICollection<GameObject> compressors;
    public ICollection<GameObject> coolent;

    // Other components on the ship which draw power from the generator.
    public ICollection<GeneratorClient> clients;

    // The power level of the generator. Currently set to an arbitrary level, perhaps you should check the scouter?
    public float powerLevel = 9001.0f;

    // The boost multiplier for each addition compression. Defaults to 10%.
    [Range(1.0f, 2.0f)]
    public float compressorBoostRatePerTick = 1.1f;

    // The penalty multiplier for running the generator without any coolent. Defaults to halving power regen.
    [Range(1.0f, 2.0f)]
    public float coolentPenaltyRatePerTick = 1.5f;

    // Assuming the generator has a motor and coolent, this is the base rate at which it will regenerate power.
    public float powerLevelRegenPerTick = 50.0f;

    public float powerLevelPenaltyPerTick = 50.0f;

    // How often the generator should recalculate the power and send out updates.
    [Range(1.0f, 1000.0f)]
    public float nextUpdate = 1000.0f / 2.0f;

    // How many milliseconds until the next update is needed.
    private float timeToNextUpdate = 0.0f;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        motors = new List<GameObject>();
        compressors = new List<GameObject>();
        coolent = new List<GameObject>();
        clients = new List<GeneratorClient>();
    }

    // Update is called once per frame
    void Update()
    {
        if (NeedsUpdate()) {
            // Update the power level before sending out further updates to the clients.
            UpdatePowerLevel();
            UpdateClients();
            Debug.Log("Generator Power Level: " + powerLevel);
        }
    }


    private bool NeedsUpdate() {
        if (timeToNextUpdate > 0) { 
            timeToNextUpdate -= Time.deltaTime;
            return false;
        }
        else { 
            Debug.Log("Generator update triggered");
            timeToNextUpdate = nextUpdate;
            return true;
        }
    }

    void UpdatePowerLevel() {
        // Running without coolent or a motor will prevent the generator from regenerating power.
        float coolentBoost = coolent.Count > 0 ? 1.0f : 0.0f;
        float compressorBoost = compressors.Count * compressorBoostRatePerTick;
        float motorBoost = motors.Count > 0 ? 1.0f : 0.0f;
        float regenRate = coolentBoost * compressorBoost * motorBoost;

        // Calculate additional negative bonuses for running the machine without consumables.
        float coolentPenalty = coolent.Count == 0 ? coolentPenaltyRatePerTick : 0.0f;
        float compressorPenalty = 1.0f; // TODO: Needed?
        float motorPenalty = 1.0f; // TODO: Needed?
        float penaltyRate = coolentPenalty * compressorPenalty * motorPenalty;

        // Calculate power draw from attached clients.
        float powerDraw = 0.0f;
        foreach(GeneratorClient client in clients) { 
            powerDraw += client.IsPowered() ? client.RequiredPowerPerGeneratorTick() : 0.0f;
        }

        // Calculate the new power level.
        float powerLevelDelta = (regenRate * powerLevelRegenPerTick) - (penaltyRate * powerLevelPenaltyPerTick) - powerDraw;

        if (powerLevel <= 0.0f && powerLevelDelta > 0.0f) {
            powerLevel += powerLevelDelta;
        }
        else { 
            powerLevel += powerLevelDelta;
            powerLevel = powerLevel >= 0.0f ? powerLevel : 0.0f;
        }
    }

    #region Client update/delegation logic

    void UpdateClients() {
        if (!wasPowerLostNotificationSent && powerLevel <= 0.0f) {
            // Power was just lost and clients need to be notified.
            wasPowerLostNotificationSent = true;
            this.PowerWasLost();
        }
        else if (wasPowerLostNotificationSent && powerLevel >= 0.0f) {
            // Power was previously lost, but should be turned back on for clients.
            wasPowerLostNotificationSent = false;
            this.PowerIsBackOn();
        }
    }

    private void PowerWasLost() {
        // TODO: Handle generator-specific logic for when the power goes out.
        Debug.Log("Generator lost power");

        // Tell any attached clients that the power is out.
        foreach (var client in clients) {
            client.PowerWasLost();
        }
    }

    private void PowerIsBackOn() {
        // TODO: Handle generator-specific logic for when the power turns back on.
        Debug.Log("Generator has regained power");

        // Tell any attached clients that the power is back on.
        foreach (var client in clients) {
            client.PowerIsBackOn();
        }
    }

    #endregion

    #region gooey internal bits

    // Used for housekeeping around turning power on and off for clients.
    private bool wasPowerLostNotificationSent = false;

    #endregion
}

public interface GeneratorClient {
    // Request the client's current on/off status.
    bool IsPowered();

    // Request the required power from the generator client.
    float RequiredPowerPerGeneratorTick();

    // Notify the client that the power was lost.
    void PowerWasLost();

    // Notify clients that the power is back on.
    void PowerIsBackOn();
}
