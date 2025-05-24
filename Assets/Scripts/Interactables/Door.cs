using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interactables {
    public class Door : InteractableObject {
        //todo: dodać tutaj referencję do obiektu co potencjalnie jest w stanie to otworzyc
        public bool IsLocked;
        public bool IsOpen;

        public float IsOpenRotation;
        public float IsClosedRotation; //albo na sztywno walnac 90 stopni i dac po prostu direction jako int

        public override void DoInteract() {
            //jak są zamknięte a gracz nie ma klucza albo czegos do otwarcia => nie rób nic
            //jak są zamknięte i gracz ma klucz - odblokuj (tylko jak są zamknięte, pomiń to jak nie są)
            //jak nie są zamknięte to po prostu toggle otwarcia i zamknięcia
        }

        //todo: trzeba bedzie pomyslec o czym innym jak bedziemy chcieli klucze dodac xd
        public override string GetInteractionName() {
            return IsLocked ? "Locked" : InteractionName;
        }

        public override bool ShouldShowBindingKey() {
            return !IsLocked;
        }
    }
}
