# 339Collab

# Jira Board Link:
https://formanl.atlassian.net/jira/software/projects/CM/boards/34?atlOrigin=eyJpIjoiOTNlODI0MzMxNjc1NDdkN2E4ZWQ5MjY3YWMwY2M4YTQiLCJwIjoiaiJ9


# Figma Link: 
NOTE: You are able to move with the middle click button or by swapping the tool at the bottom to hand, you can zoom in with ctrl + scroll. If there are problems, I have screenshots of the wireframes on the Spec Document under the "Wireframes" tab for this project, Crop Collector/Farming Match.
https://www.figma.com/design/bLceplAYaAnO83HaENjU3Y/match-board-wireframe?node-id=0-1&t=l2SHWaJ0LnTNLVr0-1

# Spec Document Link:

https://docs.google.com/document/d/1111OR8xVApTQpESf5skP_-rV4vf_S8RYE48flwblzLk/edit?usp=sharing

# BGM Music - Animal Crossing New Horizon 1 PM and 2 PM Sunny

# SFX - Itch.io free packs

# Project Stack

- Unity client project: `src/UnityGame339`
- Shared C#/.NET solution: `src/Shared/Game339.sln`
- GitHub Actions workflow: `.github/workflows/dotnet-desktop.yml`

# Shared .NET Validation

The repository includes a shared .NET pipeline for the `src/Shared` solution.
On pushes and pull requests to `main`, GitHub Actions restores, builds, and tests
the shared C# projects so assignment submissions still show an explicit .NET
validation path alongside the Unity project.

You can run the same steps locally:

```bash
cd src/Shared
dotnet restore Game339.sln
dotnet build Game339.sln --configuration Release --no-restore
dotnet test Game339.sln --configuration Release --no-build
```
