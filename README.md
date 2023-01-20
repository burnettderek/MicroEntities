# MicroEntities
MicroEntities are generic CRUD (Create Read Update Delete) templates used to rapidly develop and configure multi-tiered applications.

## Creation and Configuration

Create a SQL Server data layer for object 'User' using table 'Users' for storage, and has methods Create, Select, Update, and Delete:

```sh
var dataLayer = new SqlServerSystemLayer<User>("server=[server_name];database=[database_name];Trusted_Connection=SSPI", "Users");
```

Create a validation layer for 'User' which validates the username, password, and balance fields: 

```sh
var validationLayer = new FluentValidationLayer<User>(validator =>
{
	validator.RuleFor(user => user.UserName).Length(1, 50).Matches("^[a-zA-Z'., -]*$");
	validator.RuleFor(user => user.Password).Length(1, 50).Matches("^[a-zA-Z0-9]*$");
	validator.RuleFor(user => user.Balance).GreaterThanOrEqualTo(0);
});
```

Create a layer which benchmarks the underlying layers performance and logs the time take for each operation:

```sh
var benchmarkingLayer = new BenchmarkingLayer<User>();
```

Create a public layer which maps the internal object to one used for the public web service:

```sh
var userSystem = new PublicEntitySystem<UserDto, User>();
```

Create an entire stack for object 'User', with database, benchmarking and validation:

```sh
var userSystem = new PublicEntitySystem<UserDto, User>();

userSystem
.AddLayer(new BenchmarkingLayer<User>())
.AddLayer(new FluentValidationLayer<User>(validator =>
{
	validator.RuleFor(user => user.UserName).Length(1, 50).Matches("^[a-zA-Z'., -]*$");
	validator.RuleFor(user => user.Password).Length(1, 50).Matches("^[a-zA-Z0-9]*$");
	validator.RuleFor(user => user.Balance).GreaterThanOrEqualTo(0);
}))
.AddLayer(new SqlServerSystemLayer<User>("server=[computer_name];database=[database_name];Trusted_Connection=SSPI", "Users"));
```
## Operations

Create a valid 'User' object via webservice:

```sh
[HttpPost]
public async Task<ActionResult> Create([FromBody] UserDto user)
{
	try
	{
		var result = await _userSystem.Create(user);
		return Ok(result);
	}
	catch (ArgumentException ex)
	{
		return BadRequest(ex.Message);
	}
	catch (Exception ex)
	{
		return StatusCode((int)HttpStatusCode.InternalServerError);
	}
}
```

Read a 'User' where 'Key' field is equal to 'key':

```sh
[HttpGet]
public async Task<ActionResult> Read(Guid key)
{
	try
	{
		var result = await _userSystem.Select(Where.Equal("Key", key ));
		return Ok(result);
	}
	catch (ArgumentException ex)
	{
		return BadRequest(ex.Message);
	}
	catch (Exception ex)
	{
		return StatusCode((int)HttpStatusCode.InternalServerError);
	}
}
```

Update an entire 'User' object:

```sh
[HttpPut("/edit")]
public async Task<ActionResult> EditUser([FromBody] UserDto user)
{
	try
	{
		var result = await _userSystem.Update(user, Where.Equal(nameof(user.Key), user.Key));
		return Ok(result);
	}
	catch (ArgumentException ex)
	{
		return BadRequest(ex.Message);
	}
	catch (Exception ex)
	{
		return StatusCode((int)HttpStatusCode.InternalServerError);
	}
}
```
Update a single property of the 'User' class as well as an auditing field:

```sh
[HttpPut("/edit/{key}/{property}/{value}")]
public async Task<ActionResult> Edit(string key, string property, string value)
{
	try
	{
		if (!property.Equals("Balance"))
		{
			var result = await _userSystem.Update(Set.Value(property, value).And("LastUpdatedOn", DateTime.Now), 
													Where.Equal("Key", key));
			return Ok(result);
		}
		else
		{
			var result = await _userSystem.Update(Set.Value(property, decimal.Parse(value)).And("LastUpdatedOn", DateTime.Now), 
													Where.Equal("Key", key));
			return Ok(result);
		}
	}
	catch (ArgumentException ex)
	{
		return BadRequest(ex.Message);
	}
	catch (Exception ex)
	{
		return StatusCode((int)HttpStatusCode.InternalServerError);
	}
}
```
Delete a user based on a field:

```sh
[HttpDelete]
public async Task<ActionResult> Delete(Guid value)
{
	try
	{
		await _userSystem.Delete(Where.Equal("Key", value));
		return Ok();
	}
	catch (ArgumentException ex)
	{
		return BadRequest(ex.Message);
	}
	catch (Exception ex)
	{
		return StatusCode((int)HttpStatusCode.InternalServerError);
	}
}
```


