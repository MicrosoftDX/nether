# Architecture

The rough architecture is shown below. For more detail on the analytics pipeline, see the [documentation for Nether Analytics](../analytics)

```
                         +--------+        +---------------------+           +---------------+
                         |        |        |                     |           |               |
                         |        +--------+  Identity           +-----------+               |
                         |        |        |                     |           |               |
                         |        |        +---------------------+           |               |
                         |        |                                          |               |
+----------+             |        |        +---------------------+           |               |
|          |             |        |        |                     |           |               |
|  Client  +-------------+  REST  +--------+  Player Management  +-----------+   Database    |
|  SDKs    |             |  API   |        |                     |           |               |
|          |             |        |        +---------------------+           |               |
+----+-----+             |        |                                          |               |
     |                   |        |        +---------------------+           |               |
     |                   |        |        |                     |           |               |
     |                   |        +--------+  Analytics          +-----------+               |
     |                   |        |        |                     |           |               |
     |                   |        |        +---------------------+           |               |
     |                   |        |                                          |               |
     |                   |        |        +---------------------+           |               |
     |                   |        |        |                     |           |               |
     |                   |        +--------+  Leaderboard        +-----------+               |
     |                   |        |        |                     |           |               |
     |                   +--------+        +---------------------+           +-------+-------+
     |                                                                               |
     |                                                                               |
     |                                                                               |
     |                                                                               |
     |                   +--------+        +---------------------+                   |     +------------+
     |                   |        |        |                     |                   |     |            |
     +-------------------+  Event +--------+  Analytics          +-------------------+-----+  Data Lake |
                         |  Hub   |        |  Pipeline           |                         |  Store     |
                         |        |        |                     |                         |            |
                         +--------+        +---------------------+                         +------------+

```


## Main Components

The Nether building blocks are comprised from the following components:

### Player Management

  [TO DO] 

### Leaderboards

  [TO DO]

### Identity

  [TO DO]

  
### Analytics

  [TO DO]
  
### Client SDKs

  [TO DO]












