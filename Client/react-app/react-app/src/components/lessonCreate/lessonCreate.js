import React from "react";
// import "react-widgets/styles.css";
//import Combobox from "react-widgets/Combobox";
//import { dataText } from "react-widgets/esm/Accessors";
import { useEffect } from "react";

// Путь к API серверной части
// const url = "https://localhost:7230/api/";
// return await fetch(`${url}teachers`, requestOptions);

// const TeachersField = ({ teachers, setTeachers }) => {
//   useEffect(() => {
//     const getTeachers = async () => {
//       //функция обратного вызова, которая вызывается React в подходящее время;
//         const requestOptions = {
//           method: "GET",
//         };
//         return await fetch(`${url}teachers`, requestOptions)
//           .then((response) => response.json())
//           .then(
//             (data) => {
//               // data - данные от сервера
//               console.log("Data:", data);
//               setTeachers(data); // Изменение состояния списка занятий
//             },
//             (error) => {
//               console.log(error);
//             }
//           );
//     };
//     getTeachers();
//   }, []);
// };

// getTeachers();
// // return <h1>Привет</h1>;
// return (
//   <Combobox
//     name="selectedTeacher" //имя поля
//     data={[]} //{["Red", "Yellow", "Blue", "Orange"]}
//     defaultValue="Выбранный преподаватель" //значение выбранное по умолчанию
//   />
// );

const LessonCreate = ({ user, addLesson, teachers, setTeachers }) => {
  //const Teacher = ({ teachers, setTeachers }) => {
  useEffect(() => {
    // Выполняется функция получения данных от Сервера
    const getTeachers = async () => {
      //функция обратного вызова, которая вызывается React в подходящее время;
      const requestOptions = {
        method: "GET",
      };
      return await fetch("https://localhost:7230/api/teachers", requestOptions)
        .then((response) => response.json())
        .then(
          (data) => {
            // data - данные от сервера
            console.log("Teachers:", data);
            //console.log("setTeachers:", setTeachers);
            setTeachers(data); // Изменение состояния списка занятий
          },
          (error) => {
            console.log(error);
          }
        );
    };
    getTeachers();
  }, [setTeachers]); //массив зависимостей (необязательный)

  const handleSubmit = (e) => {
    e.preventDefault();
    var value = new Date(e.target.elements.selectedDate.value); //достали дату из строки
    const val = e.target.elements.selectedTime.value.split(":");
    var hrs = +val[0] * 3600 * 1000;
    var min = +val[1] * 60 * 1000;
    value = new Date(value.getTime() + hrs + min);
    //console.log("Selected Date:", value);

    //console.log("resultDatetime = ", resultDatetime);
    // const DICK = { date: value, StudentID: 1, TeacherID: 1, TypeID: 0};
    const lesson = {
      date: value,
      student_id: user.id, //"519512e2-3129-4987-9633-c311d8a0f47e",
      teacher_id: "b418ac0f-8942-4b75-984e-b8b99de51744",
      //cost: 1500,
      type_id: 1,
      cathegory_id: 1,
      // Student: {id: 1, FirstName: "Иван", LastName: "Иванов", MiddleName: "Иванович" },
      // Teacher: { id: 1, FirstName: "Денис", LastName: "Петров", MiddleName: "Иванович" },
      // Type: { id: 1, Name: "Практика"}
    };

    console.log(JSON.stringify(lesson));
    const createLesson = async () => {
      const requestOptions = {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(lesson), //value
      };
      const response = await fetch(
        "https://localhost:7230/api/lessons/",
        requestOptions
      );

      console.log("\n", JSON.stringify(requestOptions));
      return await response.json().then(
        (data) => {
          console.log(data);
          // response.status === 201 && addLesson(data)
          if (response.ok) {
            addLesson(data);
            // e.target.elements.selectedDate.value = new String(Date.now())
            // e.target.elements.selectedTime.value = new Date(Date.now());
          }
        },
        (error) => console.log(error)
      );
    };
    createLesson();
  };
  //Teacher();
  //
  // {console.log("UserRole:", user.userRole)};
  if (user.isAuthenticated && user.userRole === "user")
    return (
      <React.Fragment>
        {console.log("User Id = ", user.id)}
        <h3>Создание новой записи на занятие</h3>
        <form onSubmit={handleSubmit}>
          <label>Дата: </label>
          <input type="date" name="selectedDate" placeholder="Введите Дату:" />
          <input type="time" name="selectedTime" placeholder="Введите Дату:" />

          {user.isAuthenticated ? <button type="submit">Создать</button> : ""}
          {/* <TeachersField/> */}
        </form>
        <h2>Преподаватели автошколы:</h2>
        {teachers.map(
          //map - вывод из массива
          (
            { id, first_name, last_name } //{} для
          ) => (
            <div className="Teacher" key={id} id={id}>
              <li>
                <strong>
                  {/* {" "} */}
                  {/* {id}:  */}
                  {first_name} {last_name}
                </strong>
              </li>
            </div>
          )
        )}
      </React.Fragment>
    );
  else return <div>
    <h2>Преподаватели автошколы:</h2>
        {teachers.map(
          //map - вывод из массива
          (
            { id, first_name, last_name } //{} для
          ) => (
            <div className="Teacher" key={id} id={id}>
              <li>
                <strong>
                  {/* {" "} */}
                  {/* {id}:  */}
                  {first_name} {last_name}
                </strong>
              </li>
            </div>
          )
        )}
  </div>
};
export default LessonCreate;
