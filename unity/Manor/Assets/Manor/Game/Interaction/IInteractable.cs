using UnityEngine;

namespace Manor.Game.Interaction
{
    /// <summary>Anything the player can act on with the single context button.</summary>
    public interface IInteractable
    {
        /// <summary>Verb shown on the prompt: "TALK", "ENTER", "TAKE".</summary>
        string Prompt { get; }

        /// <summary>Where the prompt is anchored and distance is measured from.</summary>
        Transform Anchor { get; }

        /// <summary>False hides it from the scan entirely — a shut shop, a busy NPC.</summary>
        bool IsAvailable { get; }

        void Interact(GameObject interactor);
    }
}
