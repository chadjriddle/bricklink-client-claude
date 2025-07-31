# **BrickLink API v3 Specification for.NET Client Library Generation**

## **General API Specifications**

This section outlines the foundational protocols, conventions, and global structures that govern all interactions with the BrickLink API. Adherence to these specifications is mandatory for the successful implementation of a client library.

### **1.1 Foundational Details**

The following are universal constants and requirements for all API communications.

* **API Base URL**: All API endpoints described in this document are relative to the following base URL. All requests must be directed to this root.  
  * URL: https://api.bricklink.com/api/store/v1  
* **SSL/TLS Requirement**: All communication with the API must be conducted over a secure channel. Requests made using non-secure protocols (e.g., HTTP) will be rejected. The client must enforce the use of SSL/TLS for all outgoing requests.  
* **Character Encoding**: All string data, whether in a request body, query parameter, or response body, must be encoded using UTF-8. The client must ensure that request bodies are serialized with this encoding and that responses are deserialized accordingly.

### **1.2 Standard Data Conventions**

The API employs strict data typing and formatting rules that must be respected to ensure data integrity.

* **Timestamp Format**: All date and time values exchanged with the API are represented as strings formatted according to the ISO 8601 standard. The format includes milliseconds and timezone information, for example, 2013-12-01T18:05:46.123Z. To preserve timezone information and prevent conversion errors, it is imperative that these fields are mapped to the .NET DateTimeOffset type within the client library's data models. Using DateTime is insufficient and may lead to data corruption.  
* **Financial Precision**: The API handles all financial values as fixed-point numbers with a precision of four decimal places. When submitting financial data (e.g., prices, costs), any value with more than four decimal places will be rounded up by the API. To prevent floating-point arithmetic errors common with binary floating-point types, all monetary and financial fields must be mapped to the .NET decimal type. Use of double or float for these values is strictly prohibited.

### **1.3 Global Response Structure**

Every response from the API, regardless of success or failure, conforms to a standardized wrapper structure. This consistency allows for the creation of a generic response handling mechanism within the client library.  
A generic ApiResponse\<T\> class should be implemented to deserialize all incoming JSON responses. This class will contain a metadata object and a generic data payload. This design enables a centralized method for sending requests, which can parse the response, check the meta object for errors, and return the strongly-typed data object if the call was successful.  
**Table 1.1: Global Response Wrapper Model (ApiResponse\<T\>)**

| Property | C\# Type | Description |
| :---- | :---- | :---- |
| meta | Meta | An object containing metadata about the response, including the status code and descriptive messages. |
| data | T | The generic data payload. This can be a single resource object, a list of resource objects, or be empty. |

The meta object is crucial for determining the outcome of an API call and is defined as follows.  
**Table 1.2: Meta Object Model (Meta)**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| code | int | The HTTP status code returned by the server for the request. |  |
| message | string | A short, machine-readable string indicating the result (e.g., "OK", "INVALID\_REQUEST\_BODY"). |  |
| description | string | A detailed, human-readable message providing context about the result or describing the nature of an error. |  |

### **1.4 Error Handling and Status Codes**

The API utilizes standard HTTP status codes to signal the outcome of a request. The client library must interpret these codes to determine its subsequent actions. A non-2xx status code signifies an error condition.  
The recommended error handling pattern for the client library is to throw a custom exception, such as BrickLinkApiException, for any response with a non-2xx status code. This exception object should be populated with the code, message, and description fields from the meta object in the response body, providing rich diagnostic information to the consumer of the library.  
**Table 1.3: API Status Codes and Meanings**

| HTTP Code | Message | Meaning | Source |
| :---- | :---- | :---- | :---- |
| 200 | OK | The request was successful. |  |
| 201 | OK\_CREATED | The request was successful, and a new resource was created as a result. |  |
| 204 | OK\_NO\_CONTENT | The request was processed successfully, but there is no data to return (e.g., for a DELETE operation). |  |
| 400 | INVALID\_URI | The request URI is malformed or invalid. |  |
| 400 | INVALID\_REQUEST\_BODY | The JSON body of the request is malformed or fails validation. |  |
| 400 | PARAMETER\_MISSING\_OR\_INVALID | A required parameter is missing, or the value of a parameter is invalid. |  |
| 401 | BAD\_OAUTH\_REQUEST | The request is not properly authenticated. This may be due to an invalid signature, nonce, or token. |  |
| 403 | PERMISSION\_DENIED | The authenticated user does not have permission to access or modify the requested resource. |  |
| 404 | RESOURCE\_NOT\_FOUND | The requested resource does not exist. |  |
| 405 | METHOD\_NOT\_ALLOWED | The HTTP method used (e.g., GET, POST) is not supported for the requested resource. |  |
| 415 | UNSUPPORTED\_MEDIA\_TYPE | The content type of the request body is not supported. |  |
| 422 | RESOURCE\_UPDATE\_NOT\_ALLOWED | The resource cannot be updated in its current state. |  |
| 500 | INTERNAL\_SERVER\_ERROR | An unexpected error occurred on the server. |  |

## **Section 2: Authentication and Authorization Protocol**

The BrickLink API employs a security protocol that resembles OAuth 1.0a but with a simplified, non-standard token acquisition process. A correct implementation of this protocol is the most critical and complex aspect of building the client library.

### **2.1 Overview of the Authentication Flow**

Every request to the API must be cryptographically signed. The flow does not involve the typical three-legged OAuth process for obtaining an access token. Instead, credentials are provisioned manually through the BrickLink website, and these static credentials are then used to sign each individual API request. The primary implementation challenge is not managing a token lifecycle but correctly generating a unique signature for every HTTP call.

### **2.2 Credential Acquisition and Management**

Four distinct credential components are required to authenticate with the API. These are obtained through a two-step manual process on the BrickLink website.

1. **Consumer Key and Consumer Secret**: These are obtained by registering as an API developer on BrickLink. The Consumer Key is a public, non-changing identifier for the application, while the Consumer Secret is a confidential password that should be stored securely.  
2. **Access Token and Token Secret**: These are issued after the developer registers the static IP address(es) from which the client application will make API calls. A unique token pair is generated for each registered IP address.

**Operational Constraints**:

* **IP Whitelisting**: The API will only accept requests originating from an IP address that has been registered and associated with the provided Access Token. This is a significant operational constraint for applications hosted in environments with dynamic IP addresses.  
* **Token Expiration**: The Access Token and Token Secret do not expire. They remain valid until they are manually reissued. This necessitates secure storage of the Token Secret.

### **2.3 The Request Signing Process**

For every API request, a unique oauth\_signature must be generated and included along with other OAuth parameters. These parameters can be sent in the Authorization HTTP header (preferred) or as query string parameters.  
The signature generation process follows the principles of OAuth 1.0a:

1. **Collect Parameters**: Gather all parameters for the request. This includes:  
   * All OAuth parameters (except oauth\_signature).  
   * All query string parameters from the request URI.  
   * All form-encoded parameters from the request body (if the Content-Type is application/x-www-form-urlencoded).  
2. **Normalize and Encode**: Percent-encode every parameter name and value according to RFC3986.  
3. **Sort Parameters**: Sort the list of collected parameters alphabetically by their encoded names. If two parameters have the same name, sort them by their encoded values.  
4. **Construct Parameter String**: Concatenate the sorted parameters into a single string, with each key-value pair separated by \= and each pair separated by &.  
5. **Construct Signature Base String**: Create the signature base string by concatenating the following, separated by &:  
   * The uppercase HTTP method of the request (e.g., GET, POST).  
   * The percent-encoded base URL of the request (the URL without query string or fragment).  
   * The percent-encoded parameter string from the previous step.  
6. **Construct Signing Key**: The key for the HMAC-SHA1 algorithm is a string created by concatenating the percent-encoded Consumer Secret and the percent-encoded Token Secret, separated by an &.  
7. **Generate Signature**: Compute the HMAC-SHA1 hash of the signature base string using the signing key. The resulting hash must be Base64-encoded. This final string is the value for the oauth\_signature parameter.

### **2.4 Implementation Logic for an Authentication Handler**

This complex signing logic should be encapsulated from the end-user of the client library. The recommended approach in.NET is to implement a custom System.Net.Http.DelegatingHandler.  
This handler will be injected into the HttpClient pipeline. It will intercept every outgoing HttpRequestMessage and perform the following actions:

1. Generate a new oauth\_nonce (a unique random string) and oauth\_timestamp (the current Unix epoch time in seconds) for the request.  
2. Execute the signing process described in section 2.3 to generate the oauth\_signature.  
3. Construct the Authorization header string, containing all the required OAuth parameters.  
4. Add the Authorization header to the HttpRequestMessage before it is sent over the network.

This design abstracts the authentication complexity entirely, allowing the developer to make API calls as if they were unauthenticated (e.g., await client.GetAsync("/orders")), with the handler managing the signing process transparently.  
**Table 2.1: OAuth 1.0a Request Parameters**

| Parameter Name | C\# Type | Description | Note | Source |
| :---- | :---- | :---- | :---- | :---- |
| oauth\_consumer\_key | string | The public identifier for the application. | Required |  |
| oauth\_token | string | The access token associated with the user and registered IP address. | Required |  |
| oauth\_signature\_method | string | The signature algorithm used. | Must be HMAC-SHA1 |  |
| oauth\_timestamp | string | The number of seconds since the Unix epoch (January 1, 1970 00:00:00 GMT). Must be generated per request. | Required |  |
| oauth\_nonce | string | A unique, randomly generated string for each request to prevent replay attacks. | Required |  |
| oauth\_version | string | The version of the OAuth protocol being used. | Must be 1.0 |  |
| oauth\_signature | string | The Base64-encoded HMAC-SHA1 signature generated for the request. | Required |  |

## **Section 3: Push Notification Service (Webhooks)**

The BrickLink API provides a push notification mechanism, commonly known as webhooks, as an alternative to polling for resource changes. This allows an application to receive real-time updates for specific events.

### **3.1 Registration of Callback URLs**

To receive notifications, a developer must register one or more callback URLs in their account settings on the BrickLink website. When a subscribed event occurs, the BrickLink server will send an HTTP POST request to each registered URL. The client application must expose a public endpoint capable of receiving these POST requests.

### **3.2 Notification Triggers and Event Types**

Notifications are triggered by events related to the user's store activity. The known event types are:

* **Order**: A notification is sent when a new order is received, when a buyer updates an order's status, or when the items within an order are modified (added or deleted).  
* **Message**: A notification is sent when a new message is received.  
* **Feedback**: A notification is sent when new feedback is posted or when a reply to existing feedback is made.

A critical consideration for implementation is the API's explicit warning that "delivery of all events is not guaranteed". This implies that the webhook system should not be treated as a perfectly reliable, transactional messaging queue. For applications that require absolute data consistency (e.g., inventory synchronization, accounting), a robust architecture must use webhooks for immediate, low-latency updates but also incorporate a periodic reconciliation process. This background process should poll the relevant API endpoints (e.g., GET /orders, GET /inventories) to fetch the complete state and correct any discrepancies caused by missed notifications.

### **3.3 Inbound Notification Payload Structure**

The body of the inbound HTTP POST request from BrickLink will contain a JSON array of notification objects. Each object in the array represents a single event that has occurred. The client library should provide a model to deserialize these notifications.  
**Table 3.1: Push Notification Model (Notification)**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| event\_type | NotificationType (Enum) | The type of event that triggered the notification. Recommended enum values: Order, Message, Feedback. |  |
| resource\_id | long | The unique identifier of the resource that was affected by the event. For an Order event, this would be the order\_id. Using long ensures compatibility with large IDs. |  |
| timestamp | DateTimeOffset | The time at which the event occurred, in ISO 8601 format. |  |

## **Section 4: Resource and Endpoint Specifications**

This section provides a comprehensive definition of all known API resources, their data models, and the endpoints available for interacting with them. Each endpoint specification includes the HTTP method, URI, parameters, and request/response body structures.

### **4.1 Order Resource**

The Order resource represents a transaction between a buyer and a seller.

#### **4.1.1 Resource Models**

The Order resource is a complex object composed of several nested models.  
**Table 4.1.1: Order Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| order\_id | long | The unique identifier for the order. |  |
| date\_ordered | DateTimeOffset | The timestamp when the order was created. |  |
| seller\_name | string | The BrickLink username of the seller. |  |
| store\_name | string | The name of the seller's store. |  |
| buyer\_name | string | The BrickLink username of the buyer. |  |
| buyer\_email | string | The email address of the buyer. |  |
| buyer\_order\_count | int | Total count of all orders placed by the buyer in this store, including the current order and purged orders. |  |
| require\_insurance | bool | Indicates if the buyer requested shipping insurance. |  |
| status | OrderStatus (Enum) | The current status of the order (e.g., Pending, Completed, Shipped). |  |
| is\_invoiced | bool | Indicates if an invoice has been sent for this order. |  |
| remarks | string | General remarks or comments associated with the order. |  |
| total\_count | int | The total number of items (sum of quantities) in the order. |  |
| unique\_count | int | The number of unique lots (distinct items) in the order. |  |
| payment | Payment | An object containing payment details for the order. |  |
| shipping | Shipping | An object containing shipping and address details for the order. |  |
| cost | Cost | An object containing detailed cost breakdowns for the order. |  |

**Table 4.1.2: Payment Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| method | string | The payment method selected for the order. |  |
| currency\_code | string | The ISO 4217 currency code for the payment. |  |
| date\_paid | DateTimeOffset? | The timestamp when the order was paid. Null if not paid. |  |
| status | PaymentStatus (Enum) | The payment status (e.g., None, Received, Sent). |  |

**Table 4.1.3: Shipping Model**

| Property | C\# Type | Description | Note | Source |
| :---- | :---- | :---- | :---- | :---- |
| method | string | The shipping method selected by the buyer. |  |  |
| tracking\_no | string | The tracking number(s) for the shipment. |  |  |
| tracking\_link | string | A URL to the carrier's tracking page for the shipment. | API-only field |  |
| date\_shipped | DateTimeOffset? | The timestamp when the order was shipped. Null if not shipped. | API-only field |  |
| address | Address | The shipping address for the order. |  |  |

**Table 4.1.4: Address Model**

| Property | C\# Type | Description | Note | Source |
| :---- | :---- | :---- | :---- | :---- |
| name | PersonName | An object representing the recipient's name. | Normalized field |  |
| full | string | The full, potentially unformatted, shipping address. |  |  |
| address1 | string | The first line of the address. | To be provided |  |
| address2 | string | The second line of the address. | To be provided |  |
| address3 | string | The third line of the address. | To be provided |  |
| country\_code | string | The ISO 3166-1 alpha-2 code for the country. |  |  |
| city | string | The city. | To be provided |  |
| state | string | The state, province, or region. | To be provided |  |
| postal\_code | string | The postal or ZIP code. | To be provided |  |

**Table 4.1.5: PersonName Model**

| Property | C\# Type | Description | Note | Source |
| :---- | :---- | :---- | :---- | :---- |
| full | string | The full name of the person. |  |  |
| family | string | The family name (last name). | To be provided |  |
| given | string | The given name (first name). | To be provided |  |
| middle | string | The middle name. | To be provided |  |

**Table 4.1.6: Cost Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| currency\_code | string | The ISO 4217 currency code of the transaction. |  |
| subtotal | decimal | The total price of all items in the order, excluding shipping and other costs. |  |
| grand\_total | decimal | The final total price for the order, inclusive of all costs (items, shipping, tax, etc.). |  |
| etc1 | decimal | An extra charge field (e.g., for tax, packing). |  |
| etc2 | decimal | A second extra charge field. |  |
| insurance | decimal | The cost of shipping insurance. |  |
| shipping | decimal | The cost of shipping. |  |
| credit | decimal | Any store credit applied to the order. |  |
| coupon | decimal | The discount amount from a coupon applied to the order. |  |
| vat\_rate | decimal? | The VAT percentage rate applied to the order. |  |
| vat\_amount | decimal? | The total amount of VAT included in the grand\_total. |  |
| disp\_currency\_code | string | The ISO 4217 currency code used for displaying prices to the user. |  |
| disp\_subtotal | decimal | The subtotal displayed in the user's preferred currency. |  |
| disp\_grand\_total | decimal | The grand total displayed in the user's preferred currency. |  |

**Table 4.1.7: OrderItem Model**  
*Note: The exact schema for OrderItem is not fully detailed in the provided documentation, but is returned from the Get Order Items endpoint. The following is a composite model based on related resources.*

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| inventory\_id | long | The ID of the inventory lot from which this item was sold. |  |
| item | InventoryItem | An object describing the catalog item. |  |
| color\_id | int | The color ID of the item. |  |
| color\_name | string | The color name of the item. |  |
| quantity | int | The quantity of this item included in the order. |  |
| new\_or\_used | NewOrUsed (Enum) | The condition of the item ('N' for New, 'U' for Used). |  |
| completeness | Completeness (Enum) | For sets, indicates completeness ('C' for Complete, 'B' for Incomplete, 'S' for Sealed). |  |
| unit\_price | decimal | The price per unit for this item. |  |
| disp\_unit\_price | decimal | The price per unit displayed in the user's currency. |  |
| description | string | The seller's description for this lot. |  |
| remarks | string | The seller's remarks for this lot. |  |
| weight | decimal | The weight of the item, which may override the catalog weight. |  |

#### **4.1.2 Endpoint Specifications**

* **Get Orders**  
  * **Description**: Retrieves a list of orders. By default, it retrieves orders received by the user.  
  * **Method**: GET  
  * **URI**: /orders  
  * **Query Parameters**:  
    * direction (Direction enum, Optional): Specifies whether to fetch "in" (received, default) or "out" (placed) orders.  
    * status (string, Optional): A comma-separated list of OrderStatus values to include or exclude. Prepending a status with a minus sign (-) excludes it (e.g., pending,-purged).  
    * filed (bool, Optional): Filters for filed (true) or un-filed (false, default) orders.  
  * **Response Body**: ApiResponse\<List\<Order\>\>  
* **Get Order**  
  * **Description**: Retrieves the complete details for a single order.  
  * **Method**: GET  
  * **URI**: /orders/{order\_id}  
  * **Path Parameters**:  
    * order\_id (long, Required): The unique identifier of the order to retrieve.  
  * **Response Body**: ApiResponse\<Order\>  
* **Get Order Items**  
  * **Description**: Retrieves a list of all items (lots) included in a specific order.  
  * **Method**: GET  
  * **URI**: /orders/{order\_id}/items  
  * **Path Parameters**:  
    * order\_id (long, Required): The unique identifier of the order.  
  * **Response Body**: ApiResponse\<List\<List\<OrderItem\>\>\> (Note: The response is a list of batches, where each batch is a list of items).  
* **Get Order Messages**  
  * **Description**: Retrieves all messages exchanged between the buyer and seller for a specific order.  
  * **Method**: GET  
  * **URI**: /orders/{order\_id}/messages  
  * **Path Parameters**:  
    * order\_id (long, Required): The unique identifier of the order.  
  * **Response Body**: ApiResponse\<List\<OrderMessage\>\> (Note: OrderMessage model schema is not provided and must be inferred).  
* **Get Order Feedback**  
  * **Description**: Retrieves the feedback associated with a specific order.  
  * **Method**: GET  
  * **URI**: /orders/{order\_id}/feedback  
  * **Path Parameters**:  
    * order\_id (long, Required): The unique identifier of the order.  
  * **Response Body**: ApiResponse\<List\<Feedback\>\>  
* **Update Order**  
  * **Description**: Updates editable properties of a specific order, such as shipping costs and tracking information.  
  * **Method**: PUT  
  * **URI**: /orders/{order\_id}  
  * **Path Parameters**:  
    * order\_id (long, Required): The unique identifier of the order to update.  
  * **Request Body**: An object containing a subset of Order fields. Only the following fields are updatable: cost.credit, cost.insurance, cost.etc1, cost.etc2, cost.shipping, shipping.date\_shipped, shipping.tracking\_no, shipping.tracking\_link.  
  * **Response Body**: ApiResponse\<Order\>  
* **Update Order Status**  
  * **Description**: Updates the status of a specific order.  
  * **Method**: PUT  
  * **URI**: /orders/{order\_id}/status  
  * **Path Parameters**:  
    * order\_id (long, Required): The unique identifier of the order.  
  * **Request Body**: A patch object: {"field": "status", "value": "new\_status"} where new\_status is a valid OrderStatus string.  
  * **Response Body**: ApiResponse\<Empty\> (204 No Content on success)  
* **Update Payment Status**  
  * **Description**: Updates the payment status of a specific order.  
  * **Method**: PUT  
  * **URI**: /orders/{order\_id}/payment (Note: URI is inferred from patterns in and requires validation).  
  * **Path Parameters**:  
    * order\_id (long, Required).  
  * **Request Body**: A patch object: {"field": "status", "value": "new\_status"} where new\_status is a valid PaymentStatus string.  
  * **Response Body**: ApiResponse\<Empty\>  
* **Send Drive Thru**  
  * **Description**: Sends a "Thank You, Drive Thru\!" notification to the buyer.  
  * **Method**: POST  
  * **URI**: /orders/{order\_id}/drive\_thru (Note: URI is inferred from and requires validation).  
  * **Path Parameters**: order\_id (long, Required).  
  * **Response Body**: ApiResponse\<Empty\>  
* **File/Remove NPB Alert**  
  * **Description**: Files or removes a Non-Paying Buyer (NPB) alert for an order.  
  * **Methods**: POST (to file), DELETE (to remove).  
  * **URI**: /orders/{order\_id}/npb (Note: URI is inferred from and requires validation).  
  * **Path Parameters**: order\_id (long, Required).  
  * **Response Body**: ApiResponse\<Empty\>

### **4.2 Inventory Resource**

The Inventory resource represents items listed for sale in a user's store.

#### **4.2.1 Resource Models**

**Table 4.2.1: Inventory Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| inventory\_id | long | The unique identifier for this inventory lot. |  |
| item | InventoryItem | An object describing the catalog item. |  |
| color\_id | int | The color ID of the item. |  |
| quantity | int | The number of items available in this lot. |  |
| new\_or\_used | NewOrUsed (Enum) | The condition of the items ('N' for New, 'U' for Used). |  |
| completeness | Completeness (Enum) | For sets, indicates completeness ('C' for Complete, 'B' for Incomplete, 'S' for Sealed). |  |
| unit\_price | decimal | The price per sale unit for this item. |  |
| bind\_id | int | The ID of a parent lot this lot is bound to. |  |
| description | string | A public description of the lot displayed to buyers. |  |
| remarks | string | Private remarks for the seller's reference. |  |
| bulk | int | The multiple in which this item must be purchased (e.g., a bulk value of 100 means buyers must purchase 100, 200, etc.). |  |
| is\_retain | bool | If true, the lot remains in inventory (with quantity 0\) after being sold out. |  |
| is\_stock\_room | bool | If true, the lot is in a private stockroom and not visible to buyers. |  |
| stock\_room\_id | string | The ID of the stockroom ('A', 'B', or 'C') if is\_stock\_room is true. |  |
| date\_created | DateTimeOffset | The timestamp when this lot was created. |  |

**Table 4.2.2: InventoryItem Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| no | string | The item's catalog identification number (e.g., "3001"). |  |
| name | string | The item's official name (e.g., "Brick 2 x 4"). |  |
| type | ItemType (Enum) | The type of the item (e.g., PART, SET, MINIFIG). |  |
| category\_id | int | The ID of the item's main category. |  |

#### **4.2.2 Endpoint Specifications**

* **Get Inventories**  
  * **Description**: Retrieves a list of inventories from the user's store, with optional filtering.  
  * **Method**: GET  
  * **URI**: /inventories  
  * **Query Parameters**:  
    * item\_type (string, Optional): Comma-separated list of ItemType values to include/exclude.  
    * status (string, Optional): Comma-separated list of statuses to include/exclude (e.g., Y for available, S for in stockroom).  
    * category\_id (string, Optional): Comma-separated list of category IDs to include/exclude.  
    * color\_id (string, Optional): Comma-separated list of color IDs to include/exclude.  
  * **Response Body**: ApiResponse\<List\<Inventory\>\>  
* **Get Inventory**  
  * **Description**: Retrieves details for a specific inventory lot.  
  * **Method**: GET  
  * **URI**: /inventories/{inventory\_id}  
  * **Path Parameters**:  
    * inventory\_id (long, Required): The unique identifier of the inventory lot.  
  * **Response Body**: ApiResponse\<Inventory\>  
* **Create Inventory**  
  * **Description**: Creates a new inventory lot.  
  * **Method**: POST  
  * **URI**: /inventories  
  * **Request Body**: A single Inventory object. Required fields include item.no, item.type, quantity, unit\_price, new\_or\_used. Other fields are optional.  
  * **Response Body**: ApiResponse\<Inventory\> (The created inventory with its new inventory\_id).  
* **Create Inventories (Batch)**  
  * **Description**: Creates multiple new inventory lots in a single request.  
  * **Method**: POST  
  * **URI**: /inventories  
  * **Request Body**: A JSON array of Inventory objects. Each object has the same required fields as the single create method.  
  * **Response Body**: ApiResponse\<List\<Inventory\>\>  
* **Update Inventory**  
  * **Description**: Updates properties of an existing inventory lot.  
  * **Method**: PUT  
  * **URI**: /inventories/{inventory\_id}  
  * **Path Parameters**:  
    * inventory\_id (long, Required): The ID of the lot to update.  
  * **Request Body**: An Inventory object with the fields to be updated. The quantity field can be updated with a relative value by providing a string with a \+ or \- sign (e.g., "+10", "-5").  
  * **Response Body**: ApiResponse\<Inventory\>  
* **Delete Inventory**  
  * **Description**: Deletes an inventory lot.  
  * **Method**: DELETE  
  * **URI**: /inventories/{inventory\_id}  
  * **Path Parameters**:  
    * inventory\_id (long, Required): The ID of the lot to delete.  
  * **Response Body**: ApiResponse\<Empty\> (204 No Content on success).

### **4.3 Catalog Item Resource**

This resource provides access to the public BrickLink catalog of parts, sets, minifigures, and more.

#### **4.3.1 Resource Models**

**Table 4.3.1: CatalogItem Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| no | string | The item's catalog identification number. |  |
| name | string | The item's official name. |  |
| type | ItemType (Enum) | The type of the item. |  |
| category\_id | int | The ID of the item's main category. |  |
| alternate\_no | string | An alternate identification number for the item. |  |
| image\_url | string | A URL to the primary image of the item. |  |
| thumbnail\_url | string | A URL to the thumbnail image of the item. |  |
| weight | decimal | The item's weight in grams. |  |
| dim\_x | decimal | The item's dimension on the x-axis. |  |
| dim\_y | decimal | The item's dimension on the y-axis. |  |
| dim\_z | decimal | The item's dimension on the z-axis. |  |
| year\_released | int | The year the item was first released. |  |
| description | string | A short description of the item from the catalog. |  |
| is\_obsolete | bool | Indicates if the item is considered obsolete. |  |
| language\_code | string | The language code for book or instruction items. |  |

**Table 4.3.2: SupersetEntry Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| color\_id | int | The color of the base part for which supersets were requested. |  |
| entries | List\<SupersetItem\> | A list of items that contain the base part. |  |

* **SupersetItem contains**:  
  * item (InventoryItem): The superset item.  
  * quantity (int): How many of the base part are included in this superset.  
  * appear\_as (string): How the part appears in the superset ("A": Alternate, "C": Counterpart, "E": Extra, "R": Regular).

**Table 4.3.3: SubsetEntry Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| match\_no | int | An ID for grouping alternative parts. |  |
| entries | List\<SubsetItem\> | A list of items included in the base set. |  |

* **SubsetItem contains**:  
  * item (InventoryItem): The subset item.  
  * color\_id (int): The color ID of the subset item.  
  * quantity (int): The quantity of this item included in the set.  
  * extra\_quantity (int): The quantity of this item included as an "extra" part.  
  * is\_alternate (bool): Indicates if this item is an alternative to another in the same match\_no group.

**Table 4.3.4: PriceGuide Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| item | InventoryItem | The item for which the price guide is being provided. |  |
| new\_or\_used | NewOrUsed (Enum) | The condition of the items in this price guide ('N' or 'U'). |  |
| currency\_code | string | The ISO 4217 currency code for the prices. |  |
| min\_price | decimal | The minimum price found. |  |
| max\_price | decimal | The maximum price found. |  |
| avg\_price | decimal | The average price of all listings. |  |
| qty\_avg\_price | decimal | The quantity-weighted average price. |  |
| unit\_quantity | int | The number of lots (for "stock") or number of times sold (for "sold"). |  |
| total\_quantity | int | The total number of items available (for "stock") or sold (for "sold"). |  |
| price\_detail | List\<PriceDetail\> | A list of individual price points. |  |

**Table 4.3.5: PriceDetail Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| quantity | int | The number of items in the lot. |  |
| unit\_price | decimal | The price per unit for this lot. |  |
| shipping\_available | bool? | For "stock" guide, indicates if the seller ships to your country. |  |
| seller\_country\_code | string | For "sold" guide, the seller's country code. |  |
| buyer\_country\_code | string | For "sold" guide, the buyer's country code. |  |
| date\_ordered | DateTimeOffset? | For "sold" guide, the date the order was placed. |  |

#### **4.3.2 Endpoint Specifications**

* **Get Item**  
  * **Description**: Retrieves the main catalog information for a specific item.  
  * **Method**: GET  
  * **URI**: /items/{type}/{no}  
  * **Path Parameters**:  
    * type (ItemType enum, Required): The type of the item.  
    * no (string, Required): The catalog number of the item.  
  * **Response Body**: ApiResponse\<CatalogItem\>  
* **Get Item Image**  
  * **Description**: Retrieves the URL for an item's image in a specific color.  
  * **Method**: GET  
  * **URI**: /items/{type}/{no}/images/{color\_id} (Note: URI is inferred from documentation in ).  
  * **Path Parameters**:  
    * type (ItemType enum, Required): The type of the item.  
    * no (string, Required): The catalog number of the item.  
    * color\_id (int, Required): The color ID for the desired image.  
  * **Response Body**: ApiResponse\<CatalogItem\> (The image\_url and thumbnail\_url fields will contain the color-specific URLs).  
* **Get Supersets**  
  * **Description**: Retrieves a list of sets or other items that contain the specified item.  
  * **Method**: GET  
  * **URI**: /items/{type}/{no}/supersets  
  * **Path Parameters**:  
    * type (ItemType enum, Required): The type of the item.  
    * no (string, Required): The catalog number of the item.  
  * **Query Parameters**:  
    * color\_id (int, Optional): Filters the results to supersets that contain the item in the specified color.  
  * **Response Body**: ApiResponse\<List\<SupersetEntry\>\>  
* **Get Subsets**  
  * **Description**: Retrieves a list of items that are contained within the specified item (i.e., a set's inventory).  
  * **Method**: GET  
  * **URI**: /items/{type}/{no}/subsets  
  * **Path Parameters**:  
    * type (ItemType enum, Required): The type of the item.  
    * no (string, Required): The catalog number of the item.  
  * **Query Parameters**:  
    * color\_id (int, Optional): If the base item is a PART, specifies its color.  
    * box (bool, Optional): Include the box in the subset list.  
    * instruction (bool, Optional): Include instructions in the subset list.  
    * break\_minifigs (bool, Optional): Break down minifigures into their constituent parts.  
    * break\_subsets (bool, Optional): Recursively break down subsets.  
  * **Response Body**: ApiResponse\<List\<SubsetEntry\>\>  
* **Get Price Guide**  
  * **Description**: Retrieves pricing statistics for a specified item.  
  * **Method**: GET  
  * **URI**: /items/{type}/{no}/price  
  * **Path Parameters**:  
    * type (ItemType enum, Required): The item's type.  
    * no (string, Required): The item's catalog number.  
  * **Query Parameters**:  
    * color\_id (int, Optional): The color of the item.  
    * guide\_type (string, Optional): "sold" (for last 6 months sales) or "stock" (for current items for sale, default).  
    * new\_or\_used (NewOrUsed enum, Optional): Filter by condition ('N' or 'U').  
    * country\_code (string, Optional): Filter by seller's country (ISO 3166-1 alpha-2).  
    * region (string, Optional): Filter by seller's region (e.g., "europe", "asia").  
    * currency\_code (string, Optional): Request prices in a specific currency (ISO 4217).  
  * **Response Body**: ApiResponse\<PriceGuide\>  
* **Get Known Colors**  
  * **Description**: Retrieves a list of all colors in which a specific item is known to exist.  
  * **Method**: GET  
  * **URI**: /items/{type}/{no}/colors (Note: URI is inferred from method name in ).  
  * **Path Parameters**:  
    * type (ItemType enum, Required): The item's type.  
    * no (string, Required): The item's catalog number.  
  * **Response Body**: ApiResponse\<List\<KnownColor\>\> (Note: KnownColor model schema is not provided and must be inferred, likely containing color\_id and quantity\_in\_sets).

### **4.4 Item Mapping Resource**

This utility resource provides mappings between BrickLink catalog numbers and LEGO Element IDs.

#### **4.4.1 Resource Models**

The response schema for this resource is not documented. The following model is an inference based on the endpoints' purpose and requires empirical validation.  
**Table 4.4.1: ItemMapping Model (Inferred)**

| Property | C\# Type | Description |
| :---- | :---- | :---- |
| item | InventoryItem | The BrickLink item (no, type). |
| color\_id | int | The BrickLink color ID. |
| element\_id | string | The corresponding LEGO Element ID. |

#### **4.4.2 Endpoint Specifications**

* **Get Element ID**  
  * **Description**: Retrieves the LEGO Element ID(s) for a given BrickLink Part and color. This mapping is only provided for PART type items.  
  * **Method**: GET  
  * **URI**: /item\_mapping/{type}/{no}  
  * **Path Parameters**:  
    * type (ItemType enum, Required): Must be PART.  
    * no (string, Required): The part number.  
  * **Query Parameters**:  
    * color\_id (int, Optional): The color ID of the part. If omitted, returns mappings for all colors.  
  * **Response Body**: ApiResponse\<List\<ItemMapping\>\>  
* **Get Item Number**  
  * **Description**: Retrieves the BrickLink Item Number and Color ID for a given LEGO Element ID.  
  * **Method**: GET  
  * **URI**: /item\_mapping/{element\_id}  
  * **Path Parameters**:  
    * element\_id (string, Required): The LEGO Element ID to look up.  
  * **Response Body**: ApiResponse\<List\<ItemMapping\>\>

### **4.5 Color Resource**

Provides access to the global list of colors defined in the BrickLink catalog.

#### **4.5.1 Resource Models**

**Table 4.5.1: Color Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| color\_id | int | The unique identifier for the color. |  |
| color\_name | string | The official name of the color (e.g., "Black", "Bright Green"). |  |
| color\_code | string | The HTML hex code for the color (e.g., "05131D"). |  |
| color\_type | string | The group or type of the color (e.g., "Solid", "Transparent", "Chrome"). |  |

#### **4.5.2 Endpoint Specifications**

* **Get Color List**  
  * **Description**: Retrieves a list of all colors defined in the BrickLink catalog.  
  * **Method**: GET  
  * **URI**: /colors  
  * **Response Body**: ApiResponse\<List\<Color\>\>  
* **Get Color**  
  * **Description**: Retrieves information about a specific color.  
  * **Method**: GET  
  * **URI**: /colors/{color\_id}  
  * **Path Parameters**:  
    * color\_id (int, Required): The ID of the color to retrieve.  
  * **Response Body**: ApiResponse\<Color\>

### **4.6 Category Resource**

Provides access to the hierarchy of item categories in the BrickLink catalog.

#### **4.6.1 Resource Models**

**Table 4.6.1: Category Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| category\_id | int | The unique identifier for the category. |  |
| category\_name | string | The name of the category (e.g., "Bricks", "Minifigures"). |  |
| parent\_id | int | The ID of the parent category. A value of 0 indicates a root category. |  |

#### **4.6.2 Endpoint Specifications**

* **Get Category List**  
  * **Description**: Retrieves a list of all categories defined in the BrickLink catalog.  
  * **Method**: GET  
  * **URI**: /categories  
  * **Response Body**: ApiResponse\<List\<Category\>\>  
* **Get Category**  
  * **Description**: Retrieves information about a specific category.  
  * **Method**: GET  
  * **URI**: /categories/{category\_id}  
  * **Path Parameters**:  
    * category\_id (int, Required): The ID of the category to retrieve.  
  * **Response Body**: ApiResponse\<Category\>

### **4.7 Coupon Resource**

Manages promotional coupons for a store.

#### **4.7.1 Resource Models**

**Table 4.7.1: Coupon Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| coupon\_id | long | The unique identifier for the coupon. |  |
| date\_issued | DateTimeOffset | Timestamp when the coupon was created. |  |
| date\_expire | DateTimeOffset? | Timestamp when the coupon expires. |  |
| seller\_name | string | The username of the seller who issued the coupon. |  |
| buyer\_name | string | The username of the buyer to whom the coupon was issued. |  |
| store\_name | string | The name of the seller's store. |  |
| status | CouponStatus (Enum) | Status of the coupon ('O': Open, 'A': Redeemed, 'D': Declined, 'E': Expired). |  |
| remarks | string | A description of the coupon. |  |
| order\_id | long? | The ID of the order on which the coupon was redeemed. |  |
| currency\_code | string | The ISO 4217 currency code for the discount. |  |
| disp\_currency\_code | string | The currency code for displaying the discount. |  |
| discount\_type | DiscountType (Enum) | The type of discount ('F': Fixed amount, 'S': Percentage). |  |
| applies\_to | CouponAppliesTo | An object that restricts how a percentage coupon is applied. |  |
| discount\_amount | decimal? | For fixed-amount coupons, the value of the discount. |  |
| disp\_discount\_amount | decimal? | The fixed discount amount in the display currency. |  |
| discount\_rate | decimal? | For percentage coupons, the discount rate (e.g., 10.5 for 10.5%). |  |
| max\_discount\_amount | decimal? | The maximum discount value that can be applied for a percentage coupon. |  |

**Table 4.7.2: CouponAppliesTo Model**

| Property | C\# Type | Description | Source |
| :---- | :---- | :---- | :---- |
| type | string | Restriction type ('A': Apply to specified items, 'E': Apply to all except specified items). |  |
| item\_type | ItemType (Enum) | The item type the discount applies to or is excluded from. |  |
| except\_on\_sale | bool | If true, the discount does not apply to items currently on sale. |  |

#### **4.7.2 Endpoint Specifications**

* **Get Coupons**  
  * **Description**: Retrieves a list of coupons.  
  * **Method**: GET  
  * **URI**: /coupons  
  * **Query Parameters**:  
    * direction (Direction enum, Optional): "out" (created coupons, default) or "in" (received coupons).  
    * status (string, Optional): Comma-separated list of CouponStatus values to include/exclude.  
  * **Response Body**: ApiResponse\<List\<Coupon\>\>  
* **Get Coupon**  
  * **Description**: Retrieves a specific coupon by its ID.  
  * **Method**: GET  
  * **URI**: /coupons/{coupon\_id}  
  * **Path Parameters**:  
    * coupon\_id (long, Required): The ID of the coupon.  
  * **Response Body**: ApiResponse\<Coupon\>  
* **Create Coupon**  
  * **Description**: Creates a new coupon.  
  * **Method**: POST  
  * **URI**: /coupons  
  * **Request Body**: A Coupon object with the necessary fields to define the new coupon.  
  * **Response Body**: ApiResponse\<Coupon\>  
* **Update Coupon**  
  * **Description**: Updates an existing coupon.  
  * **Method**: PUT (Note: Documentation example in uses POST, but PUT is the RESTful standard. This requires validation).  
  * **URI**: /coupons/{coupon\_id}  
  * **Path Parameters**:  
    * coupon\_id (long, Required): The ID of the coupon to update.  
  * **Request Body**: A Coupon object with the fields to be updated.  
  * **Response Body**: ApiResponse\<Coupon\>  
* **Delete Coupon**  
  * **Description**: Deletes a coupon.  
  * **Method**: DELETE  
  * **URI**: /coupons/{coupon\_id}  
  * **Path Parameters**:  
    * coupon\_id (long, Required): The ID of the coupon to delete.  
  * **Response Body**: ApiResponse\<Empty\>

### **4.8 Member and Setting Resources**

These resources provide access to member-specific information and store settings. The data models for these resources are not fully specified in the documentation and must be inferred.

#### **4.8.1 Inferred Resource Models**

* **MemberRating (Inferred)**: Likely contains fields for feedback counts (positive, neutral, negative) for a given member.  
* **MemberNote (Inferred)**: Likely contains note\_id, username, note\_text, and date\_created.  
* **ShippingMethod (Inferred)**: Likely contains method\_id, name, cost calculation rules, and applicable regions.

#### **4.8.2 Endpoint Specifications (Inferred URIs)**

The following endpoints are listed by name in the documentation, but their exact URIs and schemas are not provided. The URIs are inferred based on common RESTful patterns and require empirical validation.

* **Get Member Rating**: GET /members/{username}/rating  
* **Get/Create/Update/Delete Member Note**: GET/POST /members/{username}/note, PUT/DELETE /members/{username}/note/{note\_id}  
* **Get Shipping Methods**: GET /settings/shipping\_methods  
* **Get Shipping Method**: GET /settings/shipping\_methods/{method\_id}

### **4.9 Push Notification Polling Resource**

This resource provides a method to poll for notifications as an alternative to using webhooks.

* **Get Notifications**  
  * **Description**: Retrieves a list of all unread push notifications.  
  * **Method**: GET  
  * **URI**: /notifications  
  * **Response Body**: ApiResponse\<List\<Notification\>\> (Uses the Notification model from Table 3.1).

## **Section 5: Client Library Architectural Recommendations**

This section provides a high-level architectural blueprint for the.NET 9 C\# client library. These recommendations are designed to produce a library that is robust, intuitive, and easy to maintain.

### **5.1 Proposed C\# Namespace and Class Structure**

A logical namespace structure will organize the library's components effectively.

* BrickLink.Client: Contains the primary BrickLinkClient class.  
* BrickLink.Client.Auth: Contains the AuthenticationHandler and related classes.  
* BrickLink.Client.Models: Contains all resource model classes (Order, Inventory, CatalogItem, etc.).  
* BrickLink.Client.Enums: Contains all public enumeration types (OrderStatus, ItemType, etc.).  
* BrickLink.Client.Webhooks: Contains helpers for parsing inbound webhook notifications.

The main entry point for the library will be a single BrickLinkClient class. This class will be instantiated with the user's four credentials and will expose properties corresponding to each API resource category (e.g., client.Orders, client.Inventories), which in turn will contain the methods for interacting with that resource (e.g., client.Orders.GetAsync(12345)).

### **5.2 Mapping API Data Types to.NET Types**

To ensure data integrity and prevent common errors, a strict mapping from API data types to.NET types must be enforced.  
**Table 5.1: API to.NET Type Mapping**

| API Type | C\# Type | Justification |
| :---- | :---- | :---- |
| Fixed Point Number | decimal | Prevents floating-point rounding errors for financial calculations. |
| Timestamp | DateTimeOffset | Preserves timezone information, preventing off-by-one errors. |
| Integer | long | Accommodates potentially large unique identifiers (order\_id, inventory\_id). |
| String | string | Direct mapping for text data. |
| Boolean | bool | Direct mapping for true/false values. |
| Object | ClassName | Mapped to a corresponding C\# class (e.g., Payment, Cost). |
| List | List\<T\> | Mapped to a generic list of the corresponding C\# type. |

### **5.3 Defining Enums for Controlled Vocabularies**

The API frequently uses string literals for parameters with a fixed set of possible values (e.g., order status, item type). Hardcoding these strings in consumer code is error-prone. The client library must define C\# enum types for these controlled vocabularies to provide compile-time safety and improve usability with IntelliSense.  
**Recommended Enums**:

* OrderStatus: Pending, Updated, Processing, Ready, Paid, Packed, Shipped, Completed, Cancelled, Purged.  
* PaymentStatus: None, Received, Sent, Completed.  
* ItemType: MINIFIG, PART, SET, BOOK, GEAR, CATALOG, INSTRUCTION, UNSORTED\_LOT, ORIGINAL\_BOX.  
* NewOrUsed: N (New), U (Used).  
* Completeness: C (Complete), B (Incomplete), S (Sealed).  
* Direction: in, out.  
* CouponStatus: O (Open), A (Redeemed), D (Declined), E (Expired).  
* DiscountType: F (Fixed), S (Percentage).

For parameters that support exclusion via a minus sign (e.g., status=-Purged), method signatures should be designed to accommodate this cleanly. One approach is to accept a collection of a custom struct that contains both the enum value and a boolean Exclude flag.

### **5.4 Asynchronous Design Patterns**

All methods that perform network I/O must be implemented asynchronously using the async/await pattern. All such methods must return a Task\<T\>, where T is the expected return type. This is standard practice for modern, non-blocking.NET applications and is essential for building responsive and scalable software.

### **5.5 Configuration and Client Instantiation**

The primary constructor for the BrickLinkClient class should accept the four required credentials, ensuring that the client cannot be instantiated in an invalid state.

* **Recommended Constructor**: public BrickLinkClient(string consumerKey, string consumerSecret, string token, string tokenSecret)

This constructor will be responsible for configuring a singleton HttpClient instance for the lifetime of the client object. It will create and inject the custom AuthenticationHandler (from Section 2.4) into the HttpClient's message pipeline. This design completely encapsulates the complex authentication mechanism, providing a simple and clean API surface to the end-user.

#### **Works cited**

1\. BrickLink API, https://static.bricklink.com/alpha/default/api\_wiki.html 2\. API guide \- BrickLink, https://www.bricklink.com/v3/api.page?page=auth 3\. Getting a Consumer Key \- BrickLink, https://www.bricklink.com/v3/api.page 4\. SHA HMAC encrypting for API authentication \- Scriptable \- Automators Talk, https://talk.automators.fm/t/sha-hmac-encrypting-for-api-authentication/15387 5\. Welcome to BrickLink Store API, https://www.bricklink.com/v2/api/welcome.page 6\. BrickLink API Calls \- van der Waal Webdesign, https://www.vanderwaal.eu/mini-projecten/bricklink-api-calls 7\. API References \- BrickLink, https://www.bricklink.com/v3/api.page?page=references 8\. Get Category List \- BrickLink, https://www.bricklink.com/v3/api.page?page=get-category-list 9\. Create Store Inventory \- BrickLink, https://www.bricklink.com/v3/api.page?page=create-inventory 10\. Get Item Image \- BrickLink, https://www.bricklink.com/v3/api.page?page=get-item-image 11\. Is there a way to use Bricklink's API to see the inventory of multiple sets? \- Reddit, https://www.reddit.com/r/Bricklink/comments/1dshvix/is\_there\_a\_way\_to\_use\_bricklinks\_api\_to\_see\_the/ 12\. bricklink\_py \- A Python wrapper for the BrickLink API \- Reddit, https://www.reddit.com/r/Bricklink/comments/1j5mrse/bricklink\_py\_a\_python\_wrapper\_for\_the\_bricklink/ 13\. BrickLink API \[BrickLink\], https://www.bricklink.com/v3/api.page?page=resource-representations-coupon 14\. Get Coupons \- BrickLink, https://www.bricklink.com/v3/api.page?page=method-coupon