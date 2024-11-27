## 🚀 Enhanced Space Shooter Game 🚀

Welcome to my improved version of the **Space Shooter Game**! 🛸 This README outlines the modifications I made to enhance the gameplay experience and add exciting new mechanics. Let’s dive into the features I implemented! 🌟

---

### 🎮 What I Did

1. **🔫 Multiple Weapons with Key Combos**
   - I added **different weapons** for the player, each with unique characteristics:
     - Lasers of varying sizes.
     - Different speeds.
     - Special firing modes (e.g., triple-shot).
   - **Key Combination System**:
     - Specific key combos activate each weapon.
     - Typing `az` fires a larger laser, `xy` fires a faster laser, and `123` triggers a triple-shot.
     - Combos are configurable through the **Inspector**, making it easy to adjust weapon behaviors without changing the code.
   - This system adds depth and variety to the gameplay. 🎯

2. **🔥 Aggression Mode**
   - I introduced an **Aggression Mode** to increase the challenge:
     - **When activated**, enemies spawn more frequently, adding intensity.
     - **How it works**:
       - Spawner behavior adjusts dynamically to the aggression state.
       - Players must adapt their strategies to survive under pressure.
   - This mode adds excitement and pushes players to strategize. 🧠⚡

3. **🛑 Restricted Spacebar Shooting**
   - To balance the gameplay and encourage the use of key combos, I implemented **spacebar rate limiting**:
     - The player can only fire **3 shots per second** using the spacebar.
     - Once the limit is reached, the spacebar enters a cooldown for **1 second**.
   - This ensures thoughtful engagement with the combo system rather than relying on rapid spacebar tapping. 🕹️

---

### 🎉 Why I Made These Changes

My goals were to:
- Add **depth and variety** to the game with the key combination system.
- Increase **gameplay challenge** with the aggression mode.
- Prevent overuse of rapid spacebar firing and encourage strategic use of **different weapons**.

---

### 🧪 How to Test

1. **Run the Game**:
   - Clone the repository and open the project in Unity.
   - Play the game and test the new mechanics.

2. **Key Combination System**:
   - Type `az`, `xy`, or `123` during gameplay to test the different weapon effects.
   - Observe how lasers change in size, speed, or firing mode.

3. **Aggression Mode**:
   - Trigger aggression mode and observe the increased enemy spawn rate.

4. **Spacebar Limiting**:
   - Rapidly tap the spacebar and confirm:
     - A maximum of 3 shots per second.
     - A 1-second cooldown after reaching the limit.

---

### 🛠️ Tools and Techniques Used

- **Unity Engine**: Core development platform.
- **Git**: Version control for efficient management and tracking.
- **C# Programming**: Implemented game mechanics and optimizations.
