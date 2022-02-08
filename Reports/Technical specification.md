# Technical Specifications

###### Last Updated

##### **Monday 07 February**

<br>
<br>

### Team

[Clement Caton](https://github.com/ClementCaton), [Louis de Choulot](https://github.com/Louis-de-Lavenne-de-Choulot), [Ivan Molnar](https://github.com/ivan-molnar), [Mathieu Chaput](https://github.com/Chaput-Mathieu), [Leo Chartier](https://github.com/leo-chartier), [Theo Diancourt](https://github.com/TheoDct)

### Reviewers

[Franck Jeannin](https://github.com/frje), [Robert Pickering](https://github.com/robertpi)

### Context

The context of the project is to create a library that could be used by a developer/artist to create, modify, listen to and save sounds used for musical purposes as well as do the same to an entire list of said sounds.  

[Project file](./Files/FsProject.pdf)

### Language

[**F#**](https://fsharp.org/)

### Library

[**SFML**](https://www.sfml-dev.org/index.php) <br>
[**XPlot.Plotly**](https://fslab.org/XPlot/plotly.html)

### Dotnet version

<br>

- DotNET 6.0

## Risks

- Compatibility with all of operating systems
- Won't be able to finish all the tasks

## Device Compatibility

- *MacOS*<br>
- *Windows*

### Download at

[**Github**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A)<br>
[**Nuget**](https://www.nuget.org/packages/Synthesizer/)

<br>

## Terminology

<br>

### Library

    An application programming interface (Library) is a connection between computers or between computer programs. It is a type of software interface, offering a service to other pieces of software

#### [Synthesis / Synthesizer](https://en.wikipedia.org/wiki/Synthesizer)

<br>

### Octave

<br>

    An octave (eighth) is the interval between one musical pitch and another with double its frequency. 

<br>

## Scope

<br>

    |           Scope          |
    |--------------------------|
    | Library                  |
    | Read/Write wav/mp3 files |
    | Playing Sounds           |
    | Modifying Sound          |
    | Applying filters         |
    | Creating musical notes   |
    | Console application      |

## Out of scope

<br>


    |       Out of scope       |
    |--------------------------|
    | Long term support        |

## Test Plan

<br>

We will do TDD all the way through the project to keep our code working well.

<br>

## User Access

<br>

The client will have access to a library containing all the functions necessary to use the features of a synthesizer.