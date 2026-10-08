using StoneForge;
using StoneForge.GameItems;

namespace ExampleMod.Items;

// Armour of our own: the game's Linen Shirt (its stats, sounds and how it's worn) dyed blue - its pictures in
// the inventory, on the ground, on the character (a female version too) and on the corpse are ours, each
// made from the game's own and the same size (the worn ones: 48x40 frames, standing / two-handed / resting).
public class ExampleShirt : LinenShirt
{
    public ExampleShirt() : base("Example Shirt")
    {
        Description = ExampleText.Get("items.exampleshirt.a_linen_shirt_dyed_blue_in");
        InventorySprite = "shirt_inv.png";
        InventoryFrames = 2;
        LootSprite = "shirt_loot.png";
        WornSprite = "shirt_worn.png";
        WornSpriteFemale = "shirt_worn_female.png";
        CorpseSprite = "shirt_corpse.png";
        Set(ArmorColumn.DEF, 3);
        Set(ArmorColumn.Magic_Resistance, 10);
        Set(ArmorColumn.Price, 300);
    }

    protected override void OnEquip(Item item) => Context.Log("Example Shirt put on");
}
