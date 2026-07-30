# AlgorithmsAndDataStructures

アルゴリズムとデータ構造の学習用リポジトリ。Unity上で実際に書いて動かしながら理解する。

## 目的
- アルゴリズム・データ構造を「自分の手で0から書く」練習
- 探索・整列などの動きをUnityで可視化して理解する

## 環境
- Unity 6（URP）

## 内容
| テーマ | 状態 |
|---|---|
| スタック `Stk<T>` | 実装済み（`StackCheck`で確認） |
| キュー `Que<T>`（自動拡張するリングバッファ） | 実装済み（`QueueCheck`で確認） |
| BFS / DFS（グリッド探索） | 進行中 |
| データ構造の確認シーン | Stack / Queue 実装済み |

## 構成
```
Assets/Scripts/
  DataStructures/        MonoBehaviour非依存の純粋C#
  Checks/                確認シーン用の軽量Controller
Assets/Scenes/
  StackCheck.unity       StackのPush / Pop / Top / Bottom確認
  QueueCheck.unity       QueueのFIFO / 折り返し / 自動拡張確認
```

NUnit・asmdefは使わず、2つの確認シーンをPlayして動作を確認する。
