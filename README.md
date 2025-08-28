# Fruity Bat

## 🎮 Let's Play
[Fruity Bat - Play on itch.io](https://berckhart.itch.io/fruity-bat)

## 🎯 Project Overview

**Fruity Bat** is a modular, cleanly-architected Match-3 puzzle game developed in Unity as a professional portfolio project.  
The project’s goal is to demonstrate modern Unity development practices, modular game architecture, and a scalable codebase with extensibility in mind.

![Match3 Gameplay Demo](https://github.com/omertekeli/Match3Portfolio/blob/1fb6e3aad7ac7387f8c18f2e5324f9c0558313fc/Animation_6.gif)

## 🧩 Core Features (MVP Scope)

- Fully playable Match-3 puzzle gameplay (swap, match, clear, refill).
- Data-driven tile and level configuration using ScriptableObjects.
- Modular architecture with clear separation of concerns.
- Event-driven system for UI, score, audio, and game logic communication.
- Simple state machine for game flow (start, playing, animation, game over)..
- Clean and readable codebase, commented and ready for further extension.
- Managing prefabs with object pooling

### UML Diagram
``` mermaid
graph TD
  subgraph Core
    I(InitLoader)
    GM(GameManager)
    AM(AudioManager)
    UIM(UIManager)
    LM(LevelManager)
    SL(SceneLoader)
    IS(InputSystem)
    SS(ScoreSystem)
    GS(GoalSystem)
  end

  subgraph Systems
    Board(BoardSystem)
    Level(LevelSystem)
  end

  subgraph Board Sub Systems
    BF(BoardFactory)
    PF(PieceFactory)
    MF(MatchFindSystem)
    MPS(MoveProcessorSystem)
    RF(RefillSystem)
  end

  subgraph UI
    UIBase(Base)
    UIComp(Components)
    UIContr(Controllers)
    UIView(Views)
  end

  subgraph Services
    EB(EventBus)
    OP(ObjectPool)
    SLc(ServiceLocator)
  end

  %% relations
  GM --> AM
  GM --> UIM
  GM --> LM
  GM --> SL
  LM --> Board
  LM --> Level
  Board --> BF
  Board --> PF
  Board --> MF
  Board --> MPS
  Board --> RF

  UIM --> UIContr
  UIContr --> UIM
  UIView --> UIContr
  UIComp --> UIView
  UIBase --> UIView

  %% service access
  Core --> SLc
  Systems --> SLc

  %% infra usage
  Core --> EB
  Systems --> EB
  UI --> EB

  BF --> OP
```

### 🔧 Custom Editor Tool
Key features of the tool:
- Visual Board Editor: Allows for easy creation of the game board by simply clicking on a visual grid. 
- Flexible Ground Types: Define any cell with one click as a 'Normal' tile, a 'Hole', or a 'Generator'. 
- Rich Content Types: Populate cells with 'Random' or 'Specific' Gems, 'Obstacles', or 'Power-ups'. 
- Overlay System: Add extra layers of complexity, like 'Ice', on top of existing items to create more diverse challenges.

![Match3 Gameplay Demo](https://github.com/omertekeli/Match3Portfolio/blob/a7207101ccc48776ef2edffde8bd5756ec872d9b/Animation_5.gif)


## ✨ Extensibility & Future Work
- Special tiles (chained, frozen, locked, etc.)
- Boosters (bomb, rocket, disco ball, etc.)
- Online leaderboards and user profiles (Firebase)

**This project is published solely for demonstration and portfolio purposes.**
