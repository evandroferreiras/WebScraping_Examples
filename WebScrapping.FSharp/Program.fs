[<EntryPoint>]
let main argv = 
    printfn "%A" WebScrap.result

    System.Console.ReadKey() |> ignore
    0