namespace Synthesizer

open System
open System.IO

module API =
    
    let getNoteFreq octav note =
        CalcNoteFreq(octav, note).Output

    let getNoteFreqOffset octav note aFourFreq =
        CalcNoteFreq(octav, note, aFourFreq).Output


    let createSound freq duration waveType =
        let data = createSoundData(frequency0 = freq, duration0 = duration, bpm0 = 114) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.makeOverdrive 1. (data.create(waveType))

    let writeToWav path music =
        writeWav().Write (File.Create(path)) (music)

    let readFromWav path =
        readWav().Read (File.Open(path, FileMode.Open))

    let note duration mNote octave =
        let freq = getNoteFreq mNote octave
        createSound freq duration Sin
    
    let silence duration =
        createSound 0 duration Silence
    
    let compose sounds =
        //this is to be revisited
        sounds |> List.map(fun x -> Utility.cutCorners 3500 x) |> List.concat
    
(*    let add (jaggedArray: float[] list) =
        let size = jaggedArray |> List.map Array.length |> List.max
        let nTracks = List.length jaggedArray
        let matrix = jaggedArray |> List.map (fun L -> (List.ofArray L) @ (List.replicate (size - Array.length L) 0.))
        Array.init size (fun j -> Array.init nTracks (fun i -> matrix.[i].[j]) |> Array.sum |> (/) (float nTracks))*)
    let add sounds = Utility.add sounds

    let preview title sound =
        previewarr.chart title sound
        sound