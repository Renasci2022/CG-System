# CG-System

**此文档是为方便查阅开发规范而创建的。**

## GitFlow 分支规范

- 长期分支
  - master
    - 主分支，功能稳定的分支
  - develop
    - 开发分支，包含所有新开发功能的分支
  - release
    - 发布分支，为了在测试完全后发布新版本的分支
    - 从 develop 中产生，在发布后合并到 master 和 develop
    - 命名规范如：release/0.1
- 临时分支
  - feature
    - 功能分支，在此分支上实现某个功能
    - 从 develop 中产生，在阶段完成后合并到 develop
    - 命名规范如：feature/fast_forward
  - resource
    - 资源分支，在此分支上导入资产和编辑 Unity 场景
    - 从 develop 中产生，在阶段完成后合并到 develop，合并后删除
  - hotfix
    - 紧急修复分支，处理 master 分支上的严重问题
    - 从 master 中产生，在修复完成后合并到 master 和 develop
    - 命名规范如：hotfix/0.1.1
  - bugfix
    - 修复分支，修复日常的 bug
    - 用 rebase 提交，不要出现在整体提交记录中，合并后删除
    - 命名规范如：bugfix/#013

[参考原文及图示](https://datasift.github.io/gitflow/IntroducingGitFlow.html)

## Angular 提交规范

此项目用简化的提交规范即可，形式如下：
```
<type>(<scope>): <short summary>
<BLANK LINE>
<body>
```
### 提交类型

目前只需要用到这些：
- docs: Documentation only changes
- feat: A new feature
- fix: A bug fix
- refactor: A code change that neither fixes a bug nor adds a feature
- res: A change to Unity scenes or resources

[参考原文](https://github.com/angular/angular/blob/main/CONTRIBUTING.md#-commit-message-format)


## TODO list

### 子任务划分

- 播放显示功能
- 暂停、快进、步进等控制功能
- 数据初始化，角色立绘系统
- 动画播放
- 交互手势
- 音乐、音效；(optional) 存档
- 测试
- 开发完毕，装载剧本
