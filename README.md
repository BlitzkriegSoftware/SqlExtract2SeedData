# SqlExtract2SeedData

An update to a little utility to extract tables in SQL Server to SQL Files for Seed Data

## Usage

```text
SqlExtract2SeedData 6.2.1
Copyright c 2020-2024 Blitzkrieg Software

  -v, --verbose          Set output to verbose messages.
  -c, --connectstring    Required. Connection String
  -t, --table            Required. SQL Table To Extract Data From
  -o, --orderby          (optional) Order By Clause in the form of "order by column1, column2"
  -w, --where            (optional) Where  Clausein the form of "where (column1 = 3)"
  -n, --ntop             (optional) Top N Rows
  -a, --ascsv            Emit Tab Separated Values (.csv) Instead of Seed Data
  --help                 Display this help screen.
  --version              Display version information.
```

> Pro Tip: please use quotes (") around strings like table names, clauses, etc.

## Sample Command

```powershell
SqlExtract2SeedData.exe `
    -c "Server=.\sqlexpress;Database=Bicycle;Trusted_Connection=True;" `
    -t "store.product"
```

## Table notation

Tables can be in the form of:

* `table` => `[dbo].[table]`
* `[table]` => `[dbo].[table]`
* `schema.table` => `[schema].[table]`
* `[schema].[table]` => `[schema].[table]`

* `"table"` => `[dbo].[table]`
* `"[table]"` => `[dbo].[table]`
* `"schema.table"` => `[schema].[table]`
* `"[schema].[table]"` => `[schema].[table]`

## Where and Order By

You can use `where` and/or `order by` clauses, just like you would do in SQL server.

Please supply full valid sql clauses such as:

```powershell
SqlExtract2SeedData `
   -c "Server=.\sqlexpress;Database=Bicycle;Trusted_Connection=True;" `
   -t "store.product" `
   -w "Where [IsActive] = 1" `
   -o "Order By [CustomerId]"
```

## Top Modifier

By default, all rows are returned. If `-n` is specified a `Top N` clause is added, the rows returned are controlled by the `where` and `order by` clauses if supplied, and will be returned in "natural" order otherwise.

```powershell
SqlExtract2SeedData `
    -c "Server=.\sqlexpress;Database=Bicycle;Trusted_Connection=True;" `
    -t "store.product" `
    -n 20
```

or 

```powershell
SqlExtract2SeedData `
    -c "Server=.\sqlexpress;Database=Bicycle;Trusted_Connection=True;" `
    -t "store.product" `
    -w "Where [IsActive] = 1" `
    -o "Order By [CustomerId]" `
    -n 100
```

## As CSV

Using the flag `-a` will create a CSV file with TAB delimiters, instead of a seed data SQL Script.

To open it in Excel or Open Office, use the file, open mechanism so that you will be given a chance to set the settings in the text import wizard:

* Field Delimiter (Tab \t)
* Row Delimiter (CRLF \r\n)

# About me

**Stuart Williams**

* I Cloud. I Code. 
* <a href="mailto:stuart.t.williams@outlook.com" target="_blank">stuart.t.williams@outlook.com</a> (e-mail)
* LinkedIn: <a href="http://lnkd.in/P35kVT" target="_blank">http://lnkd.in/P35kVT</a>
* YouTube: <a href="https://www.youtube.com/user/spookdejur1962/videos" target="_blank">https://www.youtube.com/user/spookdejur1962/videos</a> 
* GitHub: <a href="https://github.com/BlitzkriegSoftware/">https://github.com/BlitzkriegSoftware/</a>
