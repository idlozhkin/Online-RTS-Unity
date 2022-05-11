using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    [HideInInspector] public bool endGame = false;
    [Header("Human")]
    [SerializeField] private GameObject BlueWizard;
    [SerializeField] private GameObject DogKnight;
    [SerializeField] private GameObject Footman;
    [SerializeField] private GameObject GreenWizard;
    [SerializeField] private GameObject Knight;
    [SerializeField] private GameObject SmallKnight;
    [Header("Monster")]
    [SerializeField] private GameObject Chest;
    [SerializeField] private GameObject Golem;
    [SerializeField] private GameObject Orc;
    [SerializeField] private GameObject Slime;
    [SerializeField] private GameObject TurtleShell;
    [Header("Undead")]
    [SerializeField] private GameObject Dragon;
    [SerializeField] private GameObject Lich;
    [SerializeField] private GameObject Skeleton;
    [SerializeField] private GameObject Sphere;

    public GameObject[,] GetUnitsArr(int id)
    {
        switch (id)
        {
            case 0:
                return new GameObject[,] {

                    { BlueWizard, BlueWizard, GreenWizard, GreenWizard, GreenWizard},
                    { SmallKnight, SmallKnight, SmallKnight, Knight, Knight},
                    { DogKnight, DogKnight, DogKnight, DogKnight, DogKnight},
                    { Footman, Footman, Footman, Footman, Footman}
                };
            case 1:
                return new GameObject[,] {

                    { Chest, Chest, Chest, Chest, Chest},
                    { Golem, Golem, Golem, Golem, Golem},
                    { Orc, Orc, Orc, Orc, Orc},
                    { Slime, Slime, Slime, TurtleShell, TurtleShell}
                };
            default:
                return new GameObject[,] {

                    { Dragon, Dragon, Dragon, Dragon, Dragon},
                    { Lich, Lich, Lich, Lich, Lich},
                    { Skeleton, Skeleton, Skeleton, Skeleton, Skeleton},
                    { Sphere, Sphere, Sphere, Sphere, Sphere}
                };
        }
    }
}