namespace EducationSystem.Application.Abstarctions.HandlingError;

public class Result
{
    public Result(bool isSuccess, Error erorr)
    {
        // if Succeed and have error OR if Fails and Doesnot have error
        if ((isSuccess && erorr != Error.None) || (!isSuccess && Error == Error.None))
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = erorr;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; } = default!;

    public static Result Success() => new(true, Error.None);

    public static Result Fail(Error error) => new(false, error);
}

// we can return any value eith result do we make Generic Resut<T> Class
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess ?
        _value!
        : throw new InvalidOperationException("Failure results can't have avalue ");

    public Result(TValue? value, bool isSuccess, Error erorr) : base(isSuccess, erorr)
    {
        _value = value;
    }
}