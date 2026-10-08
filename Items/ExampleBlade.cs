using System;
using ExampleMod.Buffs;
using StoneForge;
using StoneForge.GameItems;

namespace ExampleMod.Items;

// A weapon of our own, inheriting the game's Drifter Sword (its stats, sprites and sounds) with sharper stats, its own pictures, and some tricks - it
// mends itself while you wear it, its blows can shock (a debuff of ours, ExampleBuffs), its crits spark for
// extra damage, and a kill with it gives you Battle Focus (a buff of ours).
public class ExampleBlade : DrifterSword
{
    private readonly Shocked _shocked;
    private readonly BattleFocus _focus;
    private readonly ExampleSettings _settings;

    public ExampleBlade(Shocked shocked, BattleFocus focus, ExampleSettings settings) : base("Example Blade")
    {
        _shocked = shocked;
        _focus = focus;
        _settings = settings;
        Description = ExampleText.Get("items.exampleblade.a_blade_made_in_c_its");
        InventorySprite = "blade_inv.png";
        LootSprite = "blade_loot.png";
        Set(WeaponColumn.Slashing_Damage, 40);
        Set(WeaponColumn.CRT, 15);
        Set(WeaponColumn.Price, 999);
    }

    // Every new one comes at half its condition (to watch it mend).
    protected override void OnCreated(Item item)
    {
        item.DurabilityPercent = 50;
        Context.Log($"a new Example Blade ({item.Quality})");
    }

    protected override void OnEquip(Item item) => Context.Log($"Example Blade drawn ({item.Durability:0}/{item.MaxDurability:0})");

    protected override void OnUnequip(Item item) => Context.Log("Example Blade put away");

    // While it's on: a point back each turn.
    protected override void OnEquippedTurn(Item item)
    {
        if (item.Durability < item.MaxDurability)
            item.Durability += 1;
    }

    // Every swing (or throw) with it, whatever came of it.
    protected override void OnAttack(Item item, Attack attack)
        => Context.Log($"Example Blade: {attack.Result}, {attack.Damage:0} damage");

    // A blow that lands: now and then (its Shock chance setting) it shocks the target for 3 turns; a crit sparks
    // for 5 more (if its Sparks setting's on); a kill focuses you for 5 turns.
    protected override void OnHit(Item item, Attack attack)
    {
        if (attack.Result == AttackResult.Crit && _settings.Sparks.Value)
        {
            attack.DealExtraDamage(5);
            Fx.Play(attack.Target, Sprite.s_lighting_enchansment_startcast, new FxOptions { OffsetY = -18, Light = Draw.Rgb(255, 230, 120) });
            Context.Log("Example Blade sparks: +5 damage");
        }
        if (attack.Killed)
        {
            Context.Log("Example Blade: a kill");
            Context.Buffs.Apply(_focus, attack.Attacker, 5);
        }
        else if (Random.Shared.NextDouble() * 100 < _settings.ShockChance.Value && Context.Buffs.Apply(_shocked, attack.Target, 3, attack.Attacker) != null)
        {
            // A burst of lightning on the blow (the game's electric weapon hit), and the debuff's own aura after.
            Fx.Play(attack.Target, Sprite.s_weapondamage_electricity, new FxOptions { OffsetY = -14, Light = Draw.Rgb(110, 170, 255) });
            Context.Log("Example Blade shocks its target");
        }
    }
}
