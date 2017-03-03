# Architecture

The rough architecture is shown below. For more detail on the analytics pipeline, see the [analytics docs](analytics.md)

```
                         +--------+        +---------------------+           +---------------+
                         |        |        |                     |           |               |
                         |        +--------+  Identity           +-----------+               |
                         |        |        |                     |           |               |
                         |        |        +---------------------+           |               |
                         |        |                                          |               |
+----------+             |        |        +---------------------+           |               |
|          |             |        |        |                     |           |               |
|  Client  +-------------+        +--------+  Player Management  +-----------+               |
|  SDKs    |             |        |        |                     |           |    Database   |
|          |             |  REST  |        +---------------------+           |               |
+----+-----+             |  API   |                                          |               |
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
     +-------------------+  Event +--------+  Analytics          +-------------------+-----+  Blob      |
                         |  Hub   |        |  Pipeline           |                         |  Storage   |
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












