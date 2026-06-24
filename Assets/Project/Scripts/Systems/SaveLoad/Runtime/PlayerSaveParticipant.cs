/*
 * Arquitectura: SaveLoad/Runtime
 * Script: PlayerSaveParticipant
 * Rol: Participante unico de guardado del Player. Es el adapter Unity/Inspector y delega en secciones no-MonoBehaviour.
 * Relaciones: SaveParticipantRegistry lo ejecuta como ISaveParticipant; internamente compone transform, vitals, inventory, jetpack y mission timer.
 * Riesgo arquitectonico mitigado: un solo componente visible no significa una sola responsabilidad interna.
 * Uso como referencia: esta misma estructura puede replicarse en EnemySaveParticipant, WorldSaveParticipant o QuestSaveParticipant.
 */
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveParticipant : MonoBehaviour, ISaveParticipant
{
    [Header("Player References")]
    [SerializeField] private PlayerInputHandler player;
    [SerializeField] private HealthBehaviour health;
    [SerializeField] private InventoryController inventory;
    [SerializeField] private MovementController movement;
    [SerializeField] private MissionTimer missionTimer;

    private readonly List<ISaveSection> sections = new List<ISaveSection>();

    private void Awake()
    {
        ResolveLocalReferences();
        BuildSections();
    }

    public void Capture(GameData gameData)
    {
        EnsureSections();

        foreach (var section in sections)
            section.Capture(gameData);
    }

    public void Restore(GameData gameData, IItemResolver itemResolver)
    {
        EnsureSections();

        foreach (var section in sections)
            section.Restore(gameData, itemResolver);
    }

    private void ResolveLocalReferences()
    {
        if (player == null)
            player = GetComponent<PlayerInputHandler>();

        if (health == null)
            health = GetComponent<HealthBehaviour>();

        if (inventory == null)
            inventory = GetComponent<InventoryController>();

        if (movement == null)
            movement = GetComponent<MovementController>();

        if (missionTimer == null)
            missionTimer = GetComponent<MissionTimer>();
    }

    private void EnsureSections()
    {
        if (sections.Count > 0)
            return;

        ResolveLocalReferences();
        BuildSections();
    }

    private void BuildSections()
    {
        sections.Clear();
        sections.Add(new PlayerTransformSaveSection(player, this));
        sections.Add(new PlayerVitalsSaveSection(health, this));
        sections.Add(new PlayerInventorySaveSection(inventory, this));
        sections.Add(new PlayerJetpackSaveSection(movement, this));
        sections.Add(new PlayerMissionTimerSaveSection(missionTimer, this));
    }
}
