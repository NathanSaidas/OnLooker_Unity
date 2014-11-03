
namespace Gem
{
    public enum UIEvent
    {
        NONE,
        /// <summary>
        /// The mouse enters the bounding box of the UI
        /// </summary>
        MOUSE_ENTER,
        /// <summary>
        /// The mouse exits the bounding box of the UI
        /// </summary>
        MOUSE_EXIT,
        /// <summary>
        /// The mouse is hovering over the bounding box of the UI
        /// </summary>
        MOUSE_HOVER,
        /// <summary>
        /// The mouse is being pressed down while hovering over the bounding box of the UI
        /// </summary>
        MOUSE_DOWN,
        /// <summary>
        /// The mouse is being pressed (single frame) while hovering over the UI
        /// </summary>
        MOUSE_CLICK,
        /// <summary>
        /// The mouse is being pressed (2 frames in less than delta X) while hovering over the same UI 
        /// </summary>
        MOUSE_DOUBLE_CLICK,
        /// <summary>
        /// The UI was selected, via mouse / keyboard or xbox control
        /// </summary>
        SELECTED,
        /// <summary>
        /// The UI was unselected, via mouse / keyboard or xbox control
        /// </summary>
        UNSELECTED,
        /// <summary>
        /// The UI has had a action event while being selected.( User hits enter key (mouse/keyboard) or a button (xbox))
        /// </summary>
        ON_ACTION

    }
}