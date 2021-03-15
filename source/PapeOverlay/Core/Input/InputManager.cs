using GameOverlay.Drawing;
using GameOverlay.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeOverlay.Core.Input
{
    public static class InputManager
    {
        internal static WindowOverlay WindowOverlay;

        /// <summary>
        /// Check if the key is down
        /// </summary>
        /// <param name="key">Key checked</param>
        /// <returns>is key down</returns>
        public static bool IsKeyDown(Key key) => InputSystem.IsKeyDown((SharpDX.DirectInput.Key)key);

        /// <summary>
        /// Check if key is up
        /// </summary>
        /// <param name="key">Key checked</param>
        /// <returns>is key up</returns>
        public static bool IsKeyUp(Key key) => InputSystem.IsKeyUp((SharpDX.DirectInput.Key)key);

        /// <summary>
        /// Check if key pressed
        /// </summary>
        /// <param name="key">Key checked</param>
        /// <returns>is key pressed</returns>
        public static bool IsKeyPressed(Key key) => InputSystem.IsKeyPressed((SharpDX.DirectInput.Key)key);

        /// <summary>
        /// Check if mouse button down
        /// </summary>
        /// <param name="button">Mouse button checked</param>
        /// <returns>is mouse button down</returns>
        public static bool IsMouseButtonDown(MouseButton button) => InputSystem.IsMouseButtonDown((int)button);

        /// <summary>
        /// Check if mouse button up
        /// </summary>
        /// <param name="button">Mouse button checked</param>
        /// <returns>is mouse button up</returns>
        public static bool IsMouseButtonUp(MouseButton button) => InputSystem.IsMouseButtonUp((int)button);

        /// <summary>
        /// Check if mouse button pressed
        /// </summary>
        /// <param name="button">Mouse button checked</param>
        /// <returns>is mouse button pressed</returns>
        public static bool IsMouseButtonPressed(MouseButton button) => InputSystem.IsMouseButtonPressed((int)button);

        /// <summary>
        /// Check if mouse is moving
        /// </summary>
        /// <returns>is mouse moving</returns>
        public static bool IsMouseMoving() => InputSystem.IsMouseMoving();

        /// <summary>
        /// Get mouse movement (it's delta)
        /// </summary>
        /// <returns>Movement amount</returns>
        public static Point GetMouseMovement() => InputSystem.GetMouseMovement();

        /// <summary>
        /// Get mouse absolute position 
        /// </summary>
        /// <returns>Mouse absolute position</returns>
        public static Point GetMouseCoordinates() => InputSystem.GetMouseCoordinates();

        /// <summary>
        /// Check if a point is inside the target window
        /// </summary>
        /// <returns>is inside window</returns>
        public static bool IsScreenPointInsideWindow() => WindowOverlay.IsScreenPointInsideWindow(GetMouseCoordinates());

        /// <summary>
        /// Get mouse position relative to the target window. Check with IsScreenPointInsideWindow first, this method can return negative coordinates if it's not inside the window screen area.
        /// </summary>
        /// <returns>Mouse relative position to target window</returns>
        public static Point GetMouseCoordinatesToWindow() => WindowOverlay.ScreenPointToWindow(GetMouseCoordinates());
    }
}
