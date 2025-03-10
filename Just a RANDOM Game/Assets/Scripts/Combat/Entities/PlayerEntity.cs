using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    public float armor; // one piece (including shield) adds 6 defense
    public float satiation;
    public float stamina;
    
    public float staminaRegenDelay = 3f; // Delay before stamina starts regenerating
    public float staminaRegenSpeed = 1;
    public float satiationConvertRate = 5;

    public float hungerDamage = 10;
    public float hungerDamageInterval = 3;

    private float lastStaminaUseTime;
    private bool isRegeneratingStamina;
    private float hungerDamageTimer = 0;

    protected override void Start()
    {
        isSpawn = true;
    }

    protected override void Update()
    {
        base.Update();
        HandleStaminaRegen();
    }

    public override void TakeDamage(Damage damage)
    {
        damage.value *= (1 - armor / (100 + armor));
        base.TakeDamage(damage);

        // TODO: play OnDamage animation, belt
    }

    public void DecreaseStamina(float amount)
    {
        stamina -= amount;
        if (stamina < 0)
            Debug.LogError("PlayerEntity DecreaseStamina() is called without checking available stamina");
        lastStaminaUseTime = Time.time;
        isRegeneratingStamina = false;
    }

    private void HandleStaminaRegen()
    {
        if (Time.time - lastStaminaUseTime >= staminaRegenDelay)
        {
            isRegeneratingStamina = true;
        }

        if (isRegeneratingStamina && stamina < 100 && satiation > 0)
        {
            stamina += Time.deltaTime * satiationConvertRate * staminaRegenSpeed;
            satiation -= Time.deltaTime * staminaRegenSpeed;
            stamina = Mathf.Min(stamina, 100);
            satiation = Mathf.Max(satiation, 0);
        }

        if (satiation <= 0)
        {
            hungerDamageTimer += Time.deltaTime;
            if(hungerDamageTimer >= hungerDamageInterval)
            {
                hungerDamageTimer = 0;
                TakeDamage(new Damage(hungerDamage));
            }
        }
        else if (hungerDamageTimer != 0)
        {
            hungerDamageTimer = 0;
        }
    }

    public override void Kill()
    {
        // TODO: death animation, death screen

        transform.SetPositionAndRotation(spawnPoints[0].position, spawnPoints[0].rotation);
        //wake up anim
    }

    public override void LoadData(GameData data)
    {
        base.LoadData(data);
        if (data.entityData.ContainsKey(entityName))
        {
            armor = data.entityData[entityName].armor;
            satiation = data.entityData[entityName].satiation;
            stamina = data.entityData[entityName].stamina;
        }
    }

    public override void SaveData(GameData data)
    {
        base.SaveData(data);
        data.entityData[entityName].armor = armor;
        data.entityData[entityName].satiation = satiation;
        data.entityData[entityName].stamina = stamina;

        PlayerItemController itemController = PlayerItemController.instance;
        if (itemController.isAiming)
            data.entityData[entityName].speedMultiplier /= itemController.aimSpeed;
        if (itemController.isEating)
            data.entityData[entityName].speedMultiplier /= itemController.eatSpeed;
    }
}
