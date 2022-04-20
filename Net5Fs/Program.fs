open Expecto

open Microsoft.EntityFrameworkCore

let private mkConnStr port =
    $"Server=tcp:db.example.com,%i{port};Initial Catalog=someDb;User Id=TestUser;Password=soOoSecret"

let private dbConfigOptions (connString: string) =
    let optionsBuilder = DbContextOptionsBuilder<Entities5.ExampleContext> ()
    let asm = typeof<Entities5.ExampleContext>.Assembly.GetName().Name
    optionsBuilder
        .UseSqlServer(
            connString,
            fun x ->
                x.MigrationsHistoryTable "__MigrationHistoryForXDb"
                |> ignore
                x.MigrationsAssembly asm |> ignore
        )
        .Options


let tests =
    test "EFCore DbContextsOptions differ for different Connection Strings" {
        let connStr1 = mkConnStr 1
        let connStr2 = mkConnStr 2

        let dbOpts1 = dbConfigOptions connStr1
        let dbOpts2 = dbConfigOptions connStr2

        if dbOpts1 = dbOpts2 then
            failtest "Expected dbOpts to not be equal"

        let opts = [| dbOpts1; dbOpts2; dbOpts1|]
        let unique =
            Array.distinct opts
        Expect.hasLength unique 2 "Expected 2 unique db opts"
    }

[<EntryPoint>]
let main args = runTestsWithCLIArgs [] args tests
