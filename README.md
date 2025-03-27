# 🎴 Triple Triad - Serveur/Client Unity

## 📌 Description  
Triple Triad est un jeu de cartes stratégique inspiré du célèbre mini-jeu de Final Fantasy, développé en **Unity** avec un système **Serveur/Client** en mode *host*.  
Le jeu se joue sur une grille **3x3**, où chaque carte possède **4 puissances** (Haut, Bas, Gauche, Droite).  
Lorsqu'une carte est posée à côté d'une autre, un **duel** est déclenché entre les valeurs correspondantes.  
Celui qui a la **plus grande puissance** remporte la carte de l’adversaire.  

---

## 🚀 Fonctionnalités  
✅ **Mode multijoueur** basé sur un système *serveur-client* *(Nakama)*  
✅ **Système de duels** entre cartes adjacentes  
✅ **Gestion des tours** et des règles de capture  
✅ **Affichage dynamique** des changements de possession des cartes  
✅ **Interface utilisateur fluide et réactive**  

---

## 🎮 Règles du Jeu  
1️⃣ Chaque joueur commence avec **5 cartes**.  
2️⃣ Les joueurs jouent chacun à leur tour en **posant une carte** sur la grille **3x3**.  
3️⃣ Lorsqu'une carte est placée, ses **côtés en contact** avec des cartes adverses déclenchent un **duel** :  
   - Si la puissance de la carte posée est **supérieure** à celle de la carte adjacente ➝ la carte adverse est capturée.  
   - Sinon, rien ne change.  
4️⃣ La partie se termine quand la grille est **remplie**.  
5️⃣ Le joueur ayant le **plus de cartes à sa couleur** remporte la partie.  

---

## 🛠️ Technologies utilisées  
🔹 **Unity** - Moteur de jeu  
🔹 **Nakama** - Gestion du multijoueur  
🔹 **C#** - Développement du gameplay  
🔹 **Unity UI** - Interface utilisateur  
