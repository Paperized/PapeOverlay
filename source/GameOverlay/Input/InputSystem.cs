using GameOverlay.Drawing;
using GameOverlay.PInvoke;
using SharpDX.DirectInput;

namespace GameOverlay.Input
{
    public static class InputSystem
    {
        private static DirectInput directInput;
        private static Keyboard keyboard;
        private static Mouse mouse;

        private static KeyboardState previousKeyboardState;
        private static KeyboardState currentKeyboardState;

        private static Point mousePosition;
        private static MouseState previousMouseState;
        private static MouseState currentMouseState;

        internal static void Init()
        {
            directInput = new DirectInput();
            keyboard = new Keyboard(directInput);
            previousKeyboardState = currentKeyboardState = new KeyboardState();

            keyboard.Properties.BufferSize = 128;
            keyboard.Acquire();

            mouse = new Mouse(directInput);
            previousMouseState = currentMouseState = new MouseState();

            mouse.Properties.BufferSize = 128;
            mouse.Acquire();
        }

        internal static void SetMousePosition(Point pos)
        {
            mousePosition = pos;
        }

        internal static void UpdateState(float deltaTime)
        {
            keyboard.Poll();
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = keyboard.GetCurrentState();

            mouse.Poll();
            previousMouseState = currentMouseState;
            currentMouseState = mouse.GetCurrentState();

            UpdateMousePosition();
        }

        private static void UpdateMousePosition()
        {
            User32.GetCursorPos(out NativePoint nativePoint);
            mousePosition.X = nativePoint.X;
            mousePosition.Y = nativePoint.Y;
        }

        public static bool IsKeyDown(Key key)
        {
            return !previousKeyboardState.IsPressed(key) && currentKeyboardState.IsPressed(key);
        }

        public static bool IsKeyUp(Key key)
        {
            return previousKeyboardState.IsPressed(key) && !currentKeyboardState.IsPressed(key);
        }

        public static bool IsKeyPressed(Key key)
        {
            return previousKeyboardState.IsPressed(key) && currentKeyboardState.IsPressed(key);
        }

        public static bool IsMouseButtonDown(int index)
        {
            return !previousMouseState.Buttons[index] && currentMouseState.Buttons[index];
        }

        public static bool IsMouseButtonUp(int index)
        {
            return previousMouseState.Buttons[index] && !currentMouseState.Buttons[index];
        }

        public static bool IsMouseButtonPressed(int index)
        {
            return previousMouseState.Buttons[index] && currentMouseState.Buttons[index];
        }

        public static bool IsMouseMoving() => currentMouseState.X != 0 || currentMouseState.Y != 0;

        public static Point GetMouseMovement() => new Point(currentMouseState.X, currentMouseState.Y);

        public static Point GetMouseCoordinates() => mousePosition;

        internal static void DisposeSystem()
        {
            mouse.Dispose();
            keyboard.Dispose();
            directInput.Dispose();
        }
    }
}
