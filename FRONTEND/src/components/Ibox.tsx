import { useState, type ReactNode } from 'react'

type IboxProps = {
  title: string
  children: ReactNode
}

export function Ibox({ title, children }: IboxProps) {
  const [collapsed, setCollapsed] = useState(false)
  const [closed, setClosed] = useState(false)

  if (closed) {
    return null
  }

  return (
    <div className={`ibox float-e-margins${collapsed ? ' border-bottom' : ''}`}>
      <div className="ibox-title">
        <h5>{title}</h5>
        <div className="ibox-tools">
          <a
            className="collapse-link"
            href="#"
            onClick={(event) => {
              event.preventDefault()
              setCollapsed((value) => !value)
            }}
          >
            <i className={`fa ${collapsed ? 'fa-chevron-down' : 'fa-chevron-up'}`}></i>
          </a>
          <a
            className="close-link"
            href="#"
            onClick={(event) => {
              event.preventDefault()
              setClosed(true)
            }}
          >
            <i className="fa fa-times"></i>
          </a>
        </div>
      </div>
      {!collapsed && <div className="ibox-content">{children}</div>}
    </div>
  )
}
