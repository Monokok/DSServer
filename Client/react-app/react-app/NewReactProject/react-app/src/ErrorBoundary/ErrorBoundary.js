

import React, { Component } from "react";
import ErrorPage from './ErrorPage';

class ErrorBoundary extends Component { //определение класса компонента ErrorBoundary, который расширяет базовый класс React.Component.
  constructor(props) {// позволяет компоненту получить доступ к своим props - свойствам компонента в React
    super(props);// Вызов функций-конструкторов родительских классов.
    this.state = { hasError: false };//В конструкторе  устанавливаем начальное состояние hasError в false.
  }

  static getDerivedStateFromError(error) {
    // Update state so the next render will show the fallback UI.
    // метод, который вызывается при возникновении ошибки в дочерних компонентах. Он обновляет состояние hasError в true, чтобы указать, что произошла ошибка.
    return { hasError: true };
  }

  componentDidCatch(error, errorInfo) {//метод, который вызывается при возникновении ошибки в дочерних компонентах. Он обновляет состояние hasError в true, чтобы указать, что произошла ошибка.
    // Здесь обработка ошибки
    console.error("ErrorBoundary поймал ошибку:", error, errorInfo);
    this.setState({ hasError: true });
  }

  render() {// внутри render происходит проверка состояния hasError. Если оно равно true, отображается сообщение об ошибке, иначе - возвращаются дочерние компоненты, которые отрисовываются.
    if (this.state.hasError) {
      return <ErrorPage></ErrorPage>; // Отображение резервного контента
    }
    return this.props.children; //this.props.children возвращает дочерние элементы, переданные в компонент ErrorBoundary.
  }
}

export default ErrorBoundary;
