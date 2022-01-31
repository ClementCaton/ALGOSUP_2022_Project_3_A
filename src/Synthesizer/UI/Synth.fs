namespace Synthesizer

open System
open System.IO

module Synth =
    
    let getNoteFreq octav note =
        CalcNoteFreq(octav, note).Output

    let getNoteFreqOffset octav note aFourFreq =
        CalcNoteFreq(octav, note, aFourFreq).Output

    let Sound freq duration waveType =
        let data = SoundData(frequency0 = freq, duration0 = duration, bpm0 = 114) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.create(waveType))

    let SoundWithEnveloppe freq duration waveType sustain attack hold decay release = // time, time, time, amp, time
        let data = SoundData(frequency0 = freq, duration0 = duration, bpm0 = 114) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.createWithEnvelope waveType sustain attack hold decay release)

    let writeToWav name music =
        Directory.CreateDirectory("./Output/") |> ignore
        use stream = File.Create("./Output/" + name)
        writeWav().Write (stream) (music)

    let writeToWavWithPath path fileName music =
        Directory.CreateDirectory(path) |> ignore
        use stream = File.Create(path + fileName)
        writeWav().Write (stream) (music)

    let readFromWav name =
        readWav().Read (File.Open("./Output/"+name, FileMode.Open))

    let readFromWavWithPath path =
        readWav().Read (File.Open(path, FileMode.Open))

    let note duration mNote octave =
        let freq = getNoteFreq mNote octave
        Sound freq duration Sin

    
    let silence duration =
        Sound 0 duration Silence
    
    let compose (corner:int) sounds =
        //this is to be revisited
        sounds |> List.map(fun x -> Utility.cutCorners corner x) |> List.concat
            
    let add sounds = Utility.add sounds

    let preview title sound =
        previewarr.chart title sound
        sound

    let forAllChannels func channels =
        channels |> List.map func

    let fourier wave =
        frequencyAnalysis.fourier(wave)
