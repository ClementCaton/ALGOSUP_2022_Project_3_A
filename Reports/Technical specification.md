# Technical Specifications
##### **Tuesday 11 January**

###### Last Updated :
##### **Friday 14 January**
<br>
<br>

### Team :
Clement Caton, Louis de Choulot, Ivan Molnar, Mathieu Chaput, Leo Chartier, Theo Diancourt

### Reviewers :
Franck Jeannin, Robert Pickering

### Context : 
The context of the project is to create a library that could be used by a developer/artist to create, modify, listen to and save sounds used for musical purposes as well as do the same to an entire list of said sounds.  
[Project file](./Files/FsProject.pdf)

### Language :
[**F#**](https://fsharp.org/)
### Library :
[**SFML**](https://www.sfml-dev.org/index.php) <br>
[**XPlot.Plotly**](https://fslab.org/XPlot/plotly.html)
### Dotnet version :
dotnet 6.0


## Risks 
- Multiplatform sound playing

## Device Compatibility :
*MacOS*<br>
*Windows*

### Download at :
[**Github**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A)

## Terminology :
### API :
    An application programming interface (API) is a connection between computers or between computer programs. It is a type of software interface, offering a service to other pieces of software

#### [Synthesis / Synthesizer](https://en.wikipedia.org/wiki/Synthesizer)

### Octave :
    An octave (eighth) is the interval between one musical pitch and another with double its frequency. 

## Scope :
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
|       Out of scope       |
|--------------------------|
| Long term support        |

## Test Plan
We will do TDD all the way through the project

## User Access
The client will have access to an API containing all the necessary functions to use the functionalities of a synthesizer.
