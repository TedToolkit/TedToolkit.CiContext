---
name: read-gitlab-ci-context
description: >-
  Read predefined GitLab CI/CD variables from TedToolkit.CiContext.GitLab when building typed job, pipeline, project, or merge request logic.
---

# Read GitLab CI Context

- Import `TedToolkit.GitLab` and check `GitLabCi.PrePipeline.General.Detected` when the same tool can run outside GitLab.
- Select `PrePipeline`, `Pipeline`, or `JobOnly` according to the variable’s GitLab availability. For example, read `GitLabCi.PrePipeline.Project.Path` for project identity and `GitLabCi.JobOnly.Pipeline.Id` for the job’s pipeline.
- Handle missing values normally across job types. Use raw access only for a custom or unsupported variable, and keep secret values out of diagnostics.
