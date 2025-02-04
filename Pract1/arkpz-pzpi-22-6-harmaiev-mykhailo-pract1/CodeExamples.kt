Важливість правил оформлення коду
Приклад:
//Поганий код
fun calc(a: Int, b: Int): Int {return a+b}

//Гарний код
fun calculateSum(firstNumber: Int, secondNumber: Int): Int {
    return firstNumber + secondNumber
}

Приклад:
//Поганий код
fun prc(x:Int,y:Int):Int{x*2+y}
val numbers= listOf(1,2,3,4);for(i in numbers){println(i)}

//Гарний код
fun processValues(x: Int, y: Int): Int { // Чітка назва, правильний відступ
    return x * 2 + y
}

val numbers = listOf(1, 2, 3, 4)
for (i in numbers) { // Код розбито на логічні частини
    println(i)
}

Структура коду. Приклад:
//Погана організація:
/src
    Main.kt
    Utils.kt
    Service.kt

//Правильна організація:
/src
    /ui
        MainScreen.kt
        SettingsScreen.kt
    /utils
        StringUtils.kt
        DateUtils.kt
    /services
        UserService.kt
        AuthService.kt
У цьому прикладі кожен пакет групує файли, пов’язані з конкретною частиною програми.

Модулі в Kotlin
Наприклад:
- UI-модуль для інтерфейсу користувача
- Data-модуль для роботи з даними
- Core-модуль для спільних компонентів

Класи повинні відповідати принципу єдиної відповідальності (SRP).
Приклад:
//Поганий код (Один клас виконує кілька задач)
class UserHandler {
    fun createUser() { /* створення користувача */ }
    fun deleteUser() { /* видалення користувача */ }
    fun validateUser() { /* валідація користувача */ }
    fun logActivity() { /* логування дій */ }
}

//Правильний код (Завдання розділені між класами)
class UserService {
    fun createUser() { /* створення користувача */ }
    fun deleteUser() { /* видалення користувача */ }
}

class UserValidator {
    fun validateUser() { /* валідація користувача */ }
}

class ActivityLogger {
    fun logActivity() { /* логування дій */ }
}

Коментарі. Приклад:
//Поганий код:
// Створення списку чисел
val numbers = listOf(1, 2, 3, 4, 5)

// Виведення списку на екран
numbers.forEach { println(it) }

//Правильний код:
// Створюємо список чисел для обробки в наступних функціях
val numbers = listOf(1, 2, 3, 4, 5)

// Виводимо список на екран для перевірки
numbers.forEach { println(it) }

Використання коментарів для розділення секцій:
// =====================
// Ініціалізація змінних
// =====================
val username = "User1"
val password = "securePassword"

// =====================
// Аутентифікація користувача
// =====================
if (authenticate(username, password)) {
    println("Authentication successful")
} else {
    println("Authentication failed")
}

Форматування коду

Приклад:
//Поганий код
val longExpression = if (condition) "A very long string that exceeds the recommended line length and is hard to read" else "Short string"

//Правильний код
val longExpression = if (condition) {
    "A very long string that exceeds the recommended line length and is hard to read"
} else {
    "Short string"
}

Табуляція чи пробіли?

//Поганий код (змішані табуляції та пробіли)
fun calculate() {    
\tval result = 42 // Табуляція замість пробілів
    println(result)
}

//Правильний код (використовуються лише пробіли)
fun calculate() {
    val result = 42
    println(result)
}

Стиль Allman:
Приклад використання:
if (condition)
{
    println("Condition met")
}
else
{
    println("Condition not met")
}

Стиль K&R. Приклад використання:
if (condition) {
    println("Condition met")
} else {
    println("Condition not met")
}

У мові програмування Kotlin стандартним є використання camelCase для змінних та функції, для класів PascalCase, а для констант UPPER\_SNAKE\_CASE.
Приклад:
//Поганий код
fun calc(x: Int, y: Int): Int { return x + y }
fun data(): String { return "info" }

//Правильний код
fun calculateSum(x: Int, y: Int): Int { return x + y }
fun fetchData(): String { return "info" }

Принципи вибору імен
//Поганий код
fun a(b: Int): Int { return b * 2 } // Нічого не зрозуміло

//Правильний код
fun calculateDouble(value: Int): Int { return value * 2 }

Уникання «магічних» чисел
Приклад:
//Поганий код
fun calculateDiscount(price: Double): Double {
    return price * 0.1 // Чому 0.1?
}

//Правильний код
const val DISCOUNT_RATE = 0.1

fun calculateDiscount(price: Double): Double {
    return price * DISCOUNT_RATE
}

Коли та де використовувати коментарі
Приклад:
//Поганий код
val numberOfUsers = 10 // Оголошення змінної числа користувачів (зайве)

//Гарний код
// Використовуємо сортування \"пузирком\" через специфічні вимоги до пам'яті
fun bubbleSort(array: IntArray): IntArray {
    // Алгоритм сортування...
}

Коментарі для пояснення коду:
Ці коментарі описують, що робить конкретний фрагмент коду. Їх використовують, коли код виконує складні або нетривіальні дії.
Приклад:
// Обчислюємо середнє арифметичне списку чисел
val average = numbers.sum() / numbers.size

Коментар пояснює чому код написаний саме так, надаючи контекст рішення.
// Перевіряємо, чи користувач має доступ до адміністративної панелі
// Логіка обмежена до рівня доступу "admin" через політику компанії
if (user.accessLevel == "admin") {
    showAdminPanel()
}

Коментарі повинні пояснювати, що не можна зрозуміти без додаткових пояснень.
Приклад:
//Поганий приклад
val users = listOf("Alice", "Bob", "Charlie") // Список користувачів
users.forEach { println(it) } // Виведення кожного користувача

//Правильний код
// Виводимо список користувачів для тестування інтерфейсу
val testUsers = listOf("Alice", "Bob", "Charlie")
testUsers.forEach { println(it) }

Коментарі KDoc автоматично обробляються середовищами розробки (наприклад, IntelliJ IDEA) і можуть бути згенеровані у вигляді HTML-документації.
Приклад:
/**
 * Розраховує суму двох чисел.
 * 
 * @param a Перше число
 * @param b Друге число
 * @return Сума чисел
 */
fun calculateSum(a: Int, b: Int): Int {
    return a + b
}

Формат та структура коментарів для документації
Приклад:
/**
 * Повертає відсортований список чисел.
 *
 * @param numbers Список чисел, які потрібно відсортувати
 * @return Відсортований список чисел
 */
fun sortNumbers(numbers: List<Int>): List<Int> {
    return numbers.sorted()
}

Включення прикладів коду в документацію допомагає розробникам зрозуміти, як використовувати функції чи класи.
Приклад:
/**
 * Повертає повідомлення вітання для користувача.
 *
 * @param name Ім'я користувача
 * @return Повідомлення вітання
 *
 * Приклад:
 * ```
 * val message = greetUser("Alice")
 * println(message) // Hello, Alice!
 * ```
 */
fun greetUser(name: String): String {
    return "Hello, $name!"
}

Java використовує рекомендації, описані в Oracle's Java Code Conventions. 
Приклад:
public class UserDetails {
    private String name;
    private int age;

    public UserDetails(String name, int age) {
        this.name = name;
        this.age = age;
    }

    public String getUserInfo() {
        return name + ", " + age + " years old";
    }
}

JavaScript має різні стилі кодування, залежно від середовища розробки (Node.js, фронтенд). П
Приклад:
class UserDetails {
    constructor(name, age) {
        this.name = name;
        this.age = age;
    }

    getUserInfo() {
        return `${this.name}, ${this.age} years old`;
    }
}


