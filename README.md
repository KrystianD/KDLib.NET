KDLib.NET
======

Collection of various utilities I use in my projects.

Supports net5.0, netstandard2.1 and netstandard2.0

### Includes

#### Configuration

* Simple (and stacked) YAML-based config class,

#### Binary utils  

* Base62 encoder,
* Binary encoder (base64, base62, hex),
* Big endian helpers - `BinaryWriter`/`BinaryReader` extensions, swap bytes, reverse bits and swap structure endianness helpers,
* Native structures serialization helpers,
* `BitReader` - for reading non byte-aligned numbers from a byte stream,
  
#### JSON

* Newtonsoft.Json converters:
  * `DateFormatJsonConverter`
  * `UnixEpochMillisecondsJsonConverter`
  * `DecimalJsonConverter`
  * `AdvancedJsonDateTimeConverter` - easily customizable date parser,
  

* JSON utilities - sorting keys, sanitizing objects, cleaning empty objects,

#### Asynchronous programming

* Async utilities
  * `WaitFutureTimeout` - waiting for task completion with timeout,
  * `FastCancellableTask` - to stop waiting for non-cancellable task,
  * `WaitAll` - for easy values retrieval when waiting on multiple tasks,
  

* Async transformations
  * `TransformAsync` - transform input to output with max running tasks limit,
  * `TransformMapAsync` - mapping input to output with max running tasks limit,
  * `TransformInChunksAsync` - transform input to output in chunks with max running tasks limit,
  

* `SingleThreadSynchronizationContext` - single-threaded main event loop (similar to Python asyncio and Node.JS main loop),

#### Other utilities

* Algorithms - combinations,
* Enum traits - eg. getting enum by value,
* Hash utils - quick MD5/SHA256 hash,
* LINQ extensions - joining to string, In/NotIn checkers, grouping utils, customizable distinct,
* Reflection utils and extensions,
* Set helpers - union, intersect, etc,
* Stream extensions - read all, reading/writing structs,
* SynchronizationContext extensions - simple Post and PostWaitAsync,
* String utils - random string generation,
* Compression utils - simple gzip compress/decompress methods,
* Global extensions - Kotlin-like Let construct,
* TCP client extensions - cancellation-enabled ConnectAsync for pre .NET 5.0,
* URI extensions - getting username/password from URI,
* URL utils - URL-encoding, creating/extending URLs, parsing URIs,
* Miscellaneous - waiting for task completion with logging.