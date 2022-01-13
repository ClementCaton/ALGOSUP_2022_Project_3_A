module playMusic
    open System.IO
    open SFML.Audio
    open SFML.System

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


        
    // let cutStart (data:float[]) (sampleRate:float) time = 
    //     data[sampleRate * time - ..]

    // let cutEnd (data:float[]) (sampleRate:float) time = 
    //     data[.. - sampleRate * time]
    
    