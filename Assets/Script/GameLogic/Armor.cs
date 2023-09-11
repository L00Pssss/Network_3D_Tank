using UnityEngine;

public enum ArmorType
{
    Vehicle,
    Module
}
public class Armor : MonoBehaviour
{
    [SerializeField] private ArmorType armorType;
    [SerializeField] private Destructible destructible;
    [SerializeField] private int thickness;

    public ArmorType Type => armorType;
    public Destructible Destructible => destructible;
    
    /// <summary>
    /// Толщина.
    /// </summary>
    public int Thickness => thickness;


    public void SetDestuctible(Destructible destructible)
    {
        this.destructible = destructible;
    }

}
