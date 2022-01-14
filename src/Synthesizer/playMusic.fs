namespace Synthesizer

open System
open System.IO
open SFML.Audio
open SFML.System
open System.Diagnostics // needed to play song on MAC OS

module playMusic =

    type OS =
        | OSX            
        | Windows
        | Linux
        | Other

    let getOS = 
        match int Environment.OSVersion.Platform with
        | 4 | 128 -> Linux // 4 is the reference for Unix
        | 6       -> OSX // 6 for OSX
        | 2       -> Windows // 2 for Windows 
        | _       -> Other

    let playWithOffset (stream:Stream) offset =     //if the offset is > to the length of the music it will start from the beginning
        let music = new Music(stream)
        let timeOffset = Time.FromSeconds(offset)
        music.PlayingOffset <- timeOffset
        music.Play()
        ignore (System.Console.ReadLine())

    let play stream =
        playWithOffset stream (float32(0.))

    let playWithOffsetFromPath (filePath:string) offset =
        let stream = File.Open (filePath, FileMode.Open)
        playWithOffset stream offset 



    
    