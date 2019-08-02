using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grill : MonoBehaviour
{
   /*public class GrillAction : GameAction
   {
      private Grill grill;
      private bool nextState;
      public GrillAction(Grill grill, bool nextState)
      {
         this.grill = grill;
         this.nextState = nextState;
      }
      protected override bool CanPerform()
      {
         return true;
      }

      protected override void Perform()
      {
         if (nextState)
         {
            grill.TurnOn();
         }
         else
         {
            grill.TurnOff();
         }
      }

      public override void Inverse()
      {
         if (!nextState)
         {
            grill.TurnOn();
         }
         else
         {
            grill.TurnOff();
         }
      }
   }*/
   
   public Material offMat;
   public Material onMat;

   private bool isOn = false;

   public void TurnOn()
   {
      isOn = true;
      GetComponentInChildren<Renderer>().material = onMat;
   }
   public void TurnOff()
   {
      isOn = false;
      GetComponentInChildren<Renderer>().material = offMat;
   }
}
