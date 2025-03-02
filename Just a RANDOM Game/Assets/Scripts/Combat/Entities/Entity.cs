using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

// an entity is an object used for combat purposes
// entities have health and can be inflicted damage and status effects
abstract public class Entity : MonoBehaviour, IDataPersistence
{
    public string entityName;
    public GameObject model;
    public float maxHealth;
    public float health;
    public float baseSpeed;
    public float speedMultiplier;
    public float baseDamage;
    public float damageMultiplier;
    public List<Transform> spawnPoints;
    public float[] respawnCooldown = new float[2]; // time interval before entity is respawned, -1 if doesn't respawn
    public SortedSet<StatusEffect> activeEffects = new SortedSet<StatusEffect>();

    private GameObject entityObj;
    private bool isSpawn = false;
    private float respawnTimer = 0;

    //public GameObject model; // the GameObject that owns this entity
    //public float maxHealth; // max health of this entity
    //public float health; // how much health does this entity have?
    //public float maxShield; // max shield of thie entity
    //public float shield; // shield regenerates after 5 seconds of not taking damage, over 2 seconds (shield gets depleted before health)
    //public float speed; // the base (100%) speed of this entity
    //public float baseDamage; // the base (100%) damage of this entity
    //public float armor; // incoming damage is multiplied by 1 - armor / (100 + abs(armor)), this increases / decreases the damage value by a maximum of 100%
    //public SortedSet<StatusEffect> activeEffects = new SortedSet<StatusEffect>(); // the active status effects of this entity
    //public string name; // name of this entity
    //public SpawnPointDirector respawnPoint; // where this entity will respawn when it dies
    //public float respawnCooldown; // time before entity is respawned

    //public Entity(
    //    string entityName,
    //    GameObject entityModel,
    //    float entitySpeed,
    //    float entityDamage,
    //    float entityMaxHealth,
    //    float entityMaxShield = 0,
    //    float entityArmor = 0
    //)
    //{
    //    model = entityModel;
    //    maxHealth = health = entityMaxHealth;
    //    maxShield = shield = entityMaxShield;
    //    speed = entitySpeed;
    //    baseDamage = entityDamage;
    //    armor = entityArmor;
    //    name = entityName;
    //}

    protected virtual void Start()
    {
        if(respawnTimer <= 0)
        {
            entityObj = ObjectPoolManager.CreatePooled(entityObj, transform.position, transform.rotation);
            entityObj.transform.SetParent(transform);
            isSpawn = true;
        }
    }

    protected virtual void Update()
    {
        if (!isSpawn)
        {
            if (respawnTimer > 0)
            {
                respawnTimer -= Time.deltaTime;
            }
            else
            {
                Respawn();
            }
        }
        else
        {
            foreach (StatusEffect effect in activeEffects)
            {
                effect.IncreaseTimer(Time.deltaTime);
            }
        }
    }

    public virtual void TakeDamage(Damage damage)
    {
        health -= damage.value;

        foreach (StatusEffect effect in damage.effects)
            ApplyStatusEffect(effect);

        if (health <= 0f)
            Kill();

        // TODO: play OnDamage animation
    }

    public virtual void Kill()
    {
        isSpawn = false;

        respawnTimer = Random.Range(respawnCooldown[0], respawnCooldown[1]);
        ObjectPoolManager.DestroyPooled(entityObj);
    }

    public virtual void Respawn()
    {
        isSpawn = true;

        health = maxHealth;
        speedMultiplier = 1;
        damageMultiplier = 1;
        activeEffects = new SortedSet<StatusEffect>();

        entityObj = ObjectPoolManager.CreatePooled(entityObj, transform.position, transform.rotation);
        entityObj.transform.SetParent(transform);
    }

    public virtual void ApplyStatusEffect(StatusEffect instance)
    {
        foreach (StatusEffect effect in activeEffects)
        {
            if (effect.GetType() == instance.GetType())
            {
                effect.AddStack(instance.stack);
                return;
            }
        }

        // if the same type of status effect isnt found
        activeEffects.Add(instance);
    }

    public virtual void LoadData(GameData data)
    {
        GameData.EntityData entityData = data.entityData[entityName];

        health = entityData.health;
        transform.position = new Vector3(entityData.position[0], entityData.position[1], entityData.position[2]);
        transform.rotation = Quaternion.Euler(entityData.rotation[0], entityData.rotation[1], entityData.rotation[2]);
        activeEffects = entityData.activeEffects;
        speedMultiplier = entityData.speedMultiplier;
        damageMultiplier = entityData.damageMultiplier;
        respawnTimer = entityData.respawnTimer;
    }

    public virtual void SaveData(GameData data)
    {
        GameData.EntityData entityData = new GameData.EntityData();
        Vector3 rot = transform.rotation.eulerAngles;

        entityData.health = health;
        entityData.position = new float[] { transform.position.x, transform.position.y, transform.position.z };
        entityData.rotation = new float[] { rot.x, rot.y, rot.z };
        entityData.activeEffects = activeEffects;
        entityData.speedMultiplier = speedMultiplier;
        entityData.damageMultiplier = damageMultiplier;
        entityData.respawnTimer = respawnTimer;

        data.entityData[entityName] = entityData;
    }

    //protected Damage ApplyArmorReduction(Damage instance)
    //{
    //    return new Damage(
    //        instance.source,
    //        instance.value * (1 - armor / (100 + Mathf.Abs(armor))),
    //        instance.effects
    //    );
    //}

    //protected void ReduceTotalHealth(float damageValue)
    //{
    //    if (shield > damageValue) // shield gets depleted first
    //        shield -= damageValue;
    //    else // if shield is not enough to absorb damage, start depleting health
    //    {
    //        health -= damageValue - shield;
    //        shield = 0;
    //    }
    //}
}
